// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";
import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class Enermy extends cc.Component {

    // LIFE-CYCLE CALLBACKS:

    // onLoad () {}

    isTouch:boolean = false;
    randomTimer:number = 1;

    start () {
        cc.tween(this.node).delay(1).call(()=>{
            this.node.getComponent(cc.RigidBody).linearVelocity = cc.v2(0, Util.getRandom(-500,-100))
        })
        .start();

        let self = this;
        this.schedule(()=>{
            this.moveRandom();
        }, self.randomTimer);
    }

    moveRandom(){
        this.node.getComponent(cc.RigidBody).linearVelocity = cc.v2(Util.getRandom(100,-100),this.node.getComponent(cc.RigidBody).linearVelocity.y);
        this.randomTimer = Util.getRandomFloat(1.1, 0.5);
    }

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(!this.isTouch && !other.name.toLowerCase().includes("enermy")){
            this.isTouch = true;

            if(other.name.toLowerCase().includes("swordbody")){
                GameController.instance.scoreController.addScore(1);
                this.node.destroy();
            }else if(other.name.toLowerCase().includes("player")){
                GameController.instance.scoreController.addScore(-1);
                this.isTouch = false;
            }
        }

        if(other.name.toLowerCase().includes("despawnarea")){
            this.node.destroy();
        }
    }

    // update (dt) {}
}
