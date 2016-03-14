using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using Newtonsoft.Json;
using UniRx;
using SocialPoint.Signals;

namespace SocialPoint.Commands
{
    public class StartBreedingCommand : Command
    {
        [Inject]
        public GameDataService gameDataService { get; set; }

        [Inject]
        public BreedingCoupleIdModel coupleId { get; set; }

        [Inject]
        public NewBreedingCoupleSignal newCoupleSignal { get; set; }


        override public void Execute()
        {
            Retain();
            // create data package for breeding view
            BreedingCoupleDataModel coupleData = new BreedingCoupleDataModel();

            var monsters = gameDataService.monsters;
            var images = gameDataService.imageCache;

            // Creating left element
            coupleData.leftMonsterId = coupleId.leftMonsterId;
            coupleData.leftMonsterName = monsters[coupleId.leftMonsterId].name;
            coupleData.leftMonsterDescription = monsters[coupleId.leftMonsterId].description;
            coupleData.leftMonsterLevel = monsters[coupleId.leftMonsterId].level;
            coupleData.leftMonsterTex = images[monsters[coupleId.leftMonsterId].full_img];
            coupleData.leftMonsterType = monsters[coupleId.leftMonsterId].type;

            // Creating right element
            coupleData.rightMonsterId = coupleId.rightMonsterId;
            coupleData.rightMonsterName = monsters[coupleId.rightMonsterId].name;
            coupleData.rightMonsterDescription = monsters[coupleId.rightMonsterId].description;
            coupleData.rightMonsterLevel = monsters[coupleId.rightMonsterId].level;
            coupleData.rightMonsterTex = images[monsters[coupleId.rightMonsterId].full_img];
            coupleData.rightMonsterType = monsters[coupleId.rightMonsterId].type;


            newCoupleSignal.Dispatch(coupleData);

            Release();
            
        }

       

    }
}