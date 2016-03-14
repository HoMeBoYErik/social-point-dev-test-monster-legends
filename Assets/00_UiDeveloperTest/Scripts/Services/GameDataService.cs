using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniRx;

namespace SocialPoint
{
    public class GameDataService : MonoBehaviour, IGameDataService
    {

        // URL where to download game data from (REST API that return a JSON string)
        public static string GameDataUrl = "http://sp-mobiledistribution-000.laicosp.net/uidevtest/?lang={0}";

        // Subscribe to data stream to receive the game data list (JSON string format)
        private IObservable<string> dataStream;

        [SerializeField]
        public ReactiveDictionary<string, ElementDataModel> elements = new ReactiveDictionary<string, ElementDataModel>(); // elements image thumbs (fire, nature, air, etc...)
        [SerializeField]
        public ReactiveCollection<MonsterDataModel> monsters = new ReactiveCollection<MonsterDataModel>();  //monsters data list        

        public FloatReactiveProperty CurrentLoadProgress;
        //public FloatReactiveProperty progress;

        // Store reference counter for requests for the same url active at the same moment
        //private Dictionary<string, int> _requestCounter = new Dictionary<string, int>();
        // We store requests download progress here so the GUI can interrogate it and show progress
        [SerializeField]
        public ReactiveDictionary<string, float> _requestDownloadProgress = new ReactiveDictionary<string, float>();
        
        // We store the cached images here
        [SerializeField]
        public Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();
        // How many times we retry to download an image request after a download failure
        public int DownloadRetryTimes = 3;

       
  
        public void progressReport(float progress)
        {
            Debug.Log("Progress is " + progress);
        }

        // Load data from remote and initialize <dataStream>
        public IObservable<string> LoadData(string language, Progress<float> progress = null)
        {
            // Setup headers for request
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Content-Type"] = "text/json; charset=utf-8";            

            dataStream = ObservableWWW.Get( string.Format(GameDataUrl, language),
                                            headers,
                                            progress);
            return dataStream;
        }

        public void LoadElements(ReactiveDictionary<string, ElementDataModel> e)
        {
            elements = e;            
        }

        public void LoadMonsters(ReactiveCollection<MonsterDataModel> m)
        {
            monsters = m;
        }

        public void DownloadImages(Progress<float> pr = null)
        {
            // - each thumb and full images for monster and elements images
            monsters
                .ToObservable<MonsterDataModel>()
                .Select(m => m.full_img) // extract the urls for full images. Stream is [full_img]
                .Merge(monsters.ToObservable<MonsterDataModel>()
                    .Select(mt => mt.thumb_img)) // extract the urls for thumb images. Stream is [full_img][thumb_img]
                .Merge(elements.Values.ToObservable<ElementDataModel>()
                    .Select(e => e.img)) // extract the urls for elements images. Stream is [monsters.full_img][monsters.thumb_img][elements.img]
                .Distinct() // remove duplicated values from the known list
                .Subscribe( // now we have the full list of url for the images we want to download...
                            url =>
                            {
#if DEBUG
                                //Debug.Log("Starting download process for " + url);
#endif
                                GetWWWTexture(url, new Progress<float>(x =>
                                {                                    
                                    _requestDownloadProgress[url] = x;
                                    CurrentLoadProgress.Value = CalcGlobalProgress();
                                    if( pr != null )
                                    {
                                        pr.Report(CurrentLoadProgress.Value);   
                                    }
                                               
                                }))
                                    .Retry(DownloadRetryTimes)   // retry up to n times in case of failure
                                    .Subscribe(
                                        tex =>
                                        {
                                            this.imageCache[url] = tex; // On success - store the downloaded image to dictionary cache
#if DEBUG
                                            //Debug.Log("Ended download process for " + url);
#endif
                                            _requestDownloadProgress[url] = 1.0f;

                                            // Update current global progress
                                            CurrentLoadProgress.Value = CalcGlobalProgress();
                                            if (pr != null)
                                            {
                                                pr.Report(CurrentLoadProgress.Value);
                                            }
                                            
                                        }
                                );

                            },
                            () => { 
#if DEBUG
                               // Debug.Log("Completed download schedule of all images ");
#endif
                            }
                 );

        }    
    
        private float CalcGlobalProgress()
        {            
            float sum = 0;
            foreach (var v in _requestDownloadProgress.Values)
            {
                sum += v;
            }
            return sum / _requestDownloadProgress.Count;
        }

        // public method to download an image as an observable and get notified about his progress
        public static IObservable<Texture2D> GetWWWTexture(string url, IProgress<float> progress = null)
        {
            // convert coroutine to IObservable
            return Observable.FromCoroutine<Texture2D>((observer, cancellationToken) => GetWWWTextureCore(url, observer, progress, cancellationToken));
        }

        // core of the previous function
        // IObserver is a callback publisher
        // Note: IObserver's basic scheme is "OnNext* (OnError | Oncompleted)?" 
        static IEnumerator GetWWWTextureCore(string url, IObserver<Texture2D> observer, IProgress<float> reportProgress, CancellationToken cancellationToken)
        {
            var request = new UnityEngine.WWW(url);
            while (!request.isDone && !cancellationToken.IsCancellationRequested)
            {
                if (reportProgress != null) reportProgress.Report(request.progress);               
                yield return null;
            }

            if (cancellationToken.IsCancellationRequested) yield break;

            if (request.error != null)
            {
                observer.OnError(new Exception(request.error));
            }
            else
            {
                if (reportProgress != null) reportProgress.Report(request.progress);  
                Texture2D tex = new Texture2D(request.texture.width, request.texture.height, TextureFormat.RGBA32, false);
                request.LoadImageIntoTexture(tex);
                observer.OnNext(tex);
                observer.OnCompleted(); // IObserver needs OnCompleted after OnNext!

                // clean the request object
                request.Dispose();
                request = null;
                tex = null;
            }
        }
      
    }
}


