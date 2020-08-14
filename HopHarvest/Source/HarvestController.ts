import GameController from "./GameController";
import SpawnController from "./SpawnController";

// Learn TypeScript:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/typescript.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/reference/attributes.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/life-cycle-callbacks.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/life-cycle-callbacks.html

const {ccclass, property} = cc._decorator;

@ccclass
export default class HarvestController extends cc.Component {

    isHarvesting:boolean = false;
    @property(cc.Prefab)
    scorePrefab:cc.Prefab = null;

    harvestVeggie(veggie:cc.Node, vegType:number){
        if(!this.isHarvesting){
            veggie.getComponent(cc.BoxCollider).enabled = false;
            this.isHarvesting = true;

            GameController.instance.spawnController.getComponent(SpawnController).emptySpot.push(veggie.parent);
            GameController.instance.spawnController.getComponent(SpawnController).removeVeggieFromMap(vegType);
            //GameController.instance.scoreKeeper.addScore(vegType);

            let score = cc.instantiate(this.scorePrefab);
            score.getComponent(cc.Label).string = "+"+GameController.instance.scoreKeeper.addScore(vegType);
            veggie.addChild(score);
        }
    }

}
