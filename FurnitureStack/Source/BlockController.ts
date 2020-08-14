import GameController from "./GameController";
import MouseController from "./MouseController";

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
export default class BlockController extends cc.Component {

    isStationary:boolean = false;
    hasSpawn:boolean = false;
    starAnimShown:boolean = false;

    spawnCallBack:Function = null;

    onBeginContact(contact: cc.PhysicsContact, selfCollider: cc.PhysicsCollider, otherCollider: cc.PhysicsCollider){
        if(!GameController.instance.isGameOver){
            GameController.instance.playCollideSfx();
            cc.tween(this.node).delay(1.5).call(()=>{
                this.node.setParent(GameController.instance.container);
            }).start();
        }
        
    }

    // update (dt) {}
}
