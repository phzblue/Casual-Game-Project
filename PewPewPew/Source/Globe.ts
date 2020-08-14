// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";
import Minion from "./Minion";
import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class Globe extends cc.Component {

    @property(cc.Prefab)
    minionPrefab:cc.Prefab[] = [];

    numOfMinion:number = 10;
    globeRadius:number = 0;

    currentGlobeDirection:number = 1;

    onLoad(){
        this.globeRadius =  this.node.getContentSize().width / 2;
    }


    start () {
        for(let i = 0; i < this.numOfMinion; i++){
            this.addMinion();
        }
    }

    changeDirection(){
        this.currentGlobeDirection *= -1;
    }

    addMinion(){
        let minion:cc.Node = cc.instantiate(this.minionPrefab[Util.getRandom(3)]);
        minion.angle = -Util.getRandom(360);
        minion.setParent(this.node);
        minion.setPosition(Util.calculatePosFromAngleRadius(this.globeRadius,-minion.angle));

        cc.tween(minion)
        .then(cc.fadeIn(1))
        //.delay(1)
        .call(()=>{
            minion.getComponent(cc.CircleCollider).enabled = true;
            minion.getComponent(Minion).canMove = true;
        })
        .start();
    }

    // update (dt) {}
}
