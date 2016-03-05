using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniRx;

namespace SocialPoint
{
    public class GameDataService : MonoBehaviour
    {

        // URL where to download game data from (REST API that return a JSON string)
        public static string GameDataUrl = "http://sp-mobiledistribution-000.laicosp.net/uidevtest/?lang={0}";
        
        [SerializeField]
        public Dictionary<string, ElementDataModel> elements;     // elements image thumb (fire, nature, air, etc...
        [SerializeField]
        //public MonsterDataModel[] monsters;                       // monsters list

        public ReactiveCollection<MonsterDataModel> monsters;

        // Subscribe to data stream to receive the game data list
        public IObservable<string> dataStream;    
        

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

        public void LoadElements(Dictionary<string, ElementDataModel> e)
        {
            //elements = new Dictionary<string, ElementDataModel>(e);
            elements = e;
        }

        public void LoadMonsters(ReactiveCollection<MonsterDataModel> m)
        {

            /*anonymousIconLoader.Subscribe (_ => {                 ObservableWWW.GetAndGetBytes (IconURL)                    .Retry(3)                     .Subscribe (x=>{                      var tex = new Texture2D(32,32);                       tex.LoadImage (x);                        IconTex.Value = tex;                  } ,                       ex =>{                            Debug.Log( "Failed:" + IconURL );                         }// onError                           );            } );*/


            //monsters = (MonsterDataModel[]) m.Clone();
            //monsters = m;
            //monsters.ToObservable<MonsterDataModel>().Subscribe<MonsterDataModel>(  x => Debug.Log(x.name), 
            //List<IObservable<byte[]>> imagesBytes = new List<IObservable<byte[]>>();
            
            var o = monsters.ObserveAdd()
                .Subscribe( md => {
                    //imagesBytes.Add(ObservableWWW.GetAndGetBytes(md.Value.full_img));
                    //imagesBytes.Add(ObservableWWW.GetAndGetBytes(md.Value.thumb_img));
                    Debug.Log("Value added to collection " + md.Value.full_img);
                });
            
            //monsters = new ReactiveCollection<MonsterDataModel>(m);  //ex => Debug.LogException(ex)) ;

           /* var parallel = Observable.WhenAll(imagesBytes).Subscribe(
                img => Debug.Log("Download of images eneded " + img.Length)

                );*/

            

            foreach( MonsterDataModel monster in m )
            {
                monsters.Add(monster);
               
            }

            o.Dispose();       
        }

        // Use this for initialization
        void Start()
        {
           /* this.LoadData("en").Subscribe(
                 x => ReadData(x), // onSuccess
                 ex => Debug.LogException(ex) // onError                
                );
            * */

               

            // notifier for progress use ScheudledNotifier or new Progress<float>(/* action */)
            /*option 1 - with action */ //var pr = new Progress<float>(progressReport);
            /* option 2 - with progress notifier*/ //var progressNotifier = new ScheduledNotifier<float>();
            
            //progressNotifier.Subscribe(x => Debug.Log(x)); // write www.progress


           /*ObservableWWW.Get(GameDataUrl, headers, progress: pr )                
                .Subscribe(
                 x => ReadData(x), // onSuccess
                 ex => Debug.LogException(ex) // onError
                );*/
        }

       
    }
}


