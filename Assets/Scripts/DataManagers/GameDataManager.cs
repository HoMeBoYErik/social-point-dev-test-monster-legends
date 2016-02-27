using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SocialPoint
{
	public class GameDataManager : MonoBehaviour {

		// URL of data service that returns a JSON depending on parameter lang
		public static string ServiceURL = "http://sp-mobiledistribution-000.laicosp.net/uidevtest/?lang=";
		// JSON Object that contain the data index returned by the service
		public JSONObject GameData;

		// We store the cached  WWW requests here
		private Dictionary<string, WWW> _requestCache = new Dictionary<string, WWW>();
		// Store reference counter for requests for the same url active at the same moment
		private Dictionary<string, int> _requestCounter = new Dictionary<string, int >();
		// We store requests download progress here so the GUI can interrogate it and show progress
		public Dictionary<string, float> _requestDownloadProgress = new Dictionary<string, float>();
		// We store the cached images here
		private Dictionary<string, Texture2D> _imageCache = new Dictionary<string, Texture2D>();
		// How many times we retry to download an image request after a download failure
		public int DownloadRetryTimes = 3;

		public float TotalDownloadProgress = 0.0f;
		public float TotalStartedRequests = 0.0f;
		public float TotalCompletedRequests = 0.0f;

		public int AvailableGems = 100;

		public void Load(string language)
		{
			// TODO if has internet
			StartCoroutine("coLoad", language);

			// TODO else queue event error and try in a while
		}

		/// <summary>
		/// Coroutine to download the JSON data index with strings and image urls
		/// </summary>
		/// <returns>The load.</returns>
		/// <param name="language">Language.</param>
		IEnumerator coLoad(string language)
		{
			WWW data;
			Dictionary<string, string> headers = new Dictionary<string,string>();
			headers["Content-Type"] = "text/json; charset=utf-8";
			// query game data for the active user language
			data = new WWW(ServiceURL + language); 

			// yield until the download is done
			yield return data;

			if (!string.IsNullOrEmpty(data.error))
			{
				EventManager.Instance.QueueEvent(new ErrorEvent(ERROR_CODES.NO_INTERNET_CONNECTION, data.error));
#if _DEBUG
				Debug.Log ("[GameDataLoader]:coLoad errors " + data.error);
#endif
			}
			else
			{
				GameData = new JSONObject(data.text);				
				//EventManager.Instance.QueueEvent(new JSONEvent("LoadGameDataComplete", GameData));

				// Clean request object
				data.Dispose();
				data = null;

				// 4 Download all images to cache
				LoadImages();

				// 5 Start updating overall download progress


			}
		}

		public void UpdateDownloadProgress (Texture2D texture)
		{
			TotalCompletedRequests++;
			TotalDownloadProgress = TotalCompletedRequests / TotalStartedRequests;

			if( TotalDownloadProgress == 1 )
			{
				// Send event that all data has been downloaded and are ready to be used
				EventManager.Instance.QueueEvent(new JSONEvent("LoadGameDataComplete", GameData));
			}
		}


		// Download all images to cache
		private void LoadImages()
		{
			// string to store the reques url
			string url;
			// - each thumb for elements
			for( int i = 0; i < GameData["elements"].Count; ++i )
			{
				url = WebUtils.UnescapeString(GameData["elements"][i]["img"].str);
				StartCoroutine(  GetTexture ( url, UpdateDownloadProgress )); // change it to download complete callback to mark as done
				++this.TotalStartedRequests;
				#if _DEBUG
				Debug.Log ("[elements] Download request for : " + url);
				#endif
			}
			// - each thumb and full images for monster
			for ( int i = 0; i < GameData["monsters"].Count; ++i )
			{
				// thumb
				url = WebUtils.UnescapeString(GameData["monsters"][i]["thumb_img"].str);
				StartCoroutine(  GetTexture ( url, UpdateDownloadProgress )); // change it to download complete callback to mark as done
				++this.TotalStartedRequests;
				#if _DEBUG
				Debug.Log ("[thumbs] Download request for : " + url);
				#endif
				// full image
				url = WebUtils.UnescapeString(GameData["monsters"][i]["full_img"].str);
				StartCoroutine(  GetTexture ( url, UpdateDownloadProgress )); // change it to download complete callback to mark as done
				++this.TotalStartedRequests;
				#if _DEBUG
				Debug.Log ("[full] Download request for : " + url);
				#endif
			}
		}


		/// <summary>
		/// Download an image from web, store it to a Texture2D and store the request as well to prevent duplicates
		/// The image (Texture2D) is saved in a Dictionary where the key is the request url, so duplicated images in data
		/// are only downloaded once
		/// </summary>
		/// <returns>The 2D texture via a callback if callback != null.</returns>
		/// <param name="url">image URL</param>
		/// <param name="callback">Optional callback that receives the downloaded Texture2D</param>

		public IEnumerator GetTexture(string url, Action<Texture2D> callback)
		{			
			if (!this._imageCache.ContainsKey(url) ) 
			{
				int retryTimes = this.DownloadRetryTimes; // Number of time to retry if we get a web error
				WWW request;
				do {
					--retryTimes;
					if ( !this._requestCache.ContainsKey(url) ) 
					{
						// Create a new web request and cache is so any additional
						// calls with the same url share the same request.
						this._requestCache[url] = new WWW (url);
						// set the dowload progress for the request to 0
						this._requestDownloadProgress[url] = 0.0f;
					}
					// share the same WWW request already cached and in progress
					request = this._requestCache[url];

					// Sum 1 to request reference counter
					if ( this._requestCounter.ContainsKey(url) ) 
					{
						++this._requestCounter[url];
					}
					// create entry in the reference counter if not exists
					else
					{
						this._requestCounter[url] = 1;
					}

					// ...wait for request to complete
					while( !request.isDone)
					{
						// update download progress for the request 
						this._requestDownloadProgress[url] = request.progress;
						yield return null;
					}

					// Decrement 1 reference counter on download completed
					if ( this._requestCache.ContainsKey(url) && this._requestCache[url] == request) 
					{
						this._requestCounter[url] -= 1;
					}

				} while( request.error != null && retryTimes >= 0 );
				
				// If there are no errors add this is the first coroutine to complete the download,
				// then add the texture to the texture cache.
				if ( request.error == null && !this._imageCache.ContainsKey(url) ) 
				{
					Texture2D tex = new Texture2D(request.texture.width, request.texture.height, TextureFormat.RGBA32, false);
					request.LoadImageIntoTexture(tex);
					this._imageCache[url] = tex;
					// Be sure to dispose the request object after use, not to create orphans
					// Remove this request from the cache and dispose it if the ref counter reaches 0
					if ( this._requestCounter[url] == 0 ) 
					{
						// All the reference to this request completed
						this._requestDownloadProgress[url] = 1.0f;
						// clean the dictionary of requests and ref counter
						this._requestCache.Remove(url);
						this._requestCounter.Remove(url);
						// clean the request object
						request.Dispose();
						request = null;	
					}
					// Clean the tex object
					tex = null;
				}							
			}
			
			if (callback != null) 
			{
				// By the time we get here there is either a valid image in the cache
				// or we were not able to get the requested image.
				Texture2D texture = null;
				this._imageCache.TryGetValue (url, out texture);
				callback (texture);
			}
		
		}// end of function

		////////////

		/// Get a string by key based on current locale
		public string GetString(string key)
		{

			if( !GameData["strings"].IsNull && !GameData["strings"][key].IsNull )
			{
				return GameData["strings"][key].str;
			}
			else
			{
				return null;
			}
		}

		/////////////

	}// end of class
}// end of namespace

