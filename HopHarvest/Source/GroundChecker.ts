import GameController from "./GameController";

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
export default class GroundChecker extends cc.Component {

    @property(cc.Prefab)
    dustAnim:cc.Prefab = null;

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(!GameController.instance.isGameOver && other.name.toLowerCase().includes("grabber")){
            GameController.instance.bunnicontroller.jumped = false;
            GameController.instance.bunnicontroller.canHarvest = false;

            GameController.instance.bunnicontroller.node.parent.getComponent(cc.Animation).play("Idle");

            let dust = cc.instantiate(this.dustAnim);
            this.node.addChild(dust);
            dust.setPosition(other.node.parent.position.x,0);

            cc.tween(dust).delay(2).call(()=>{dust.destroy();})
            // dust.runAction(cc.sequence(cc.delayTime(2),cc.callFunc(()=>{
            //     dust.destroy();
            // })))
        }
    }
}
