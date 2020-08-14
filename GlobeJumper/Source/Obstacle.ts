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
export default class Obstacle extends cc.Component {

    @property()
    speed:number = 1;
    radius:number = 250;
    angle:number = 0;

    // LIFE-CYCLE CALLBACKS:

    // onLoad () {}

    start () {
        if(Util.getRandom(2) % 2 == 0){
            this.speed *= -1;
            this.node.scaleX = 1;
            console.log("here")
        }

        cc.tween(this.node)
        .delay(1.5)
        .then(cc.fadeIn(0.5))
        .call(()=>{
            this.node.getComponent(cc.BoxCollider).enabled = true;
        })
        .start();
    }

    update (dt) {
        if(!GameController.instance.isGameOver){
            this.angle += .01 * this.speed;
            let x = this.radius * Math.sin(-this.angle);
            let y = this.radius * Math.cos(-this.angle);
    
            this.node.setPosition(x,y);
        }
    }
}
