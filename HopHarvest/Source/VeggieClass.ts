import BunniController from "./BunniController";
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
export default class VeggieClass extends cc.Component {

    // onLoad () {}
    
    testing = cc.Enum({
        carrot:0,
        eggplant:1,
        tomato:2
    }); 

    @property(cc.Enum)
    vegType = this.testing.carrot;

    @property({
        type: cc.AudioClip
    })
    harvestSFX:cc.AudioClip = null;

    start(){
        //do sprouting animation

        cc.tween(this.node).delay(0.5).
        parallel(cc.scaleTo(0.5,1),cc.fadeIn(0.5),cc.jumpBy(0.5,0,0,50,2).easing(cc.easeSineIn())).
        delay(0.2).
        call(()=>{this.node.getComponent(cc.BoxCollider).enabled = true}).start();

        // this.node.runAction(cc.sequence(
        //     cc.delayTime(0.5),
        //     cc.spawn(cc.scaleTo(0.5,1),cc.fadeIn(0.5),cc.jumpBy(0.5,0,0,50,2).easing(cc.easeSineIn())),
        //     cc.delayTime(0.2),
        //     cc.callFunc(()=>{ this.node.getComponent(cc.BoxCollider).enabled = true })
        // ));
    }

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(!GameController.instance.isGameOver){

            if(other.node.parent.name.includes("Bunni") && GameController.instance.bunnicontroller.canHarvest){
                
                if(!GameController.instance.harvestController.isHarvesting){
                    
                    GameController.instance.harvestController.harvestVeggie(this.node,this.vegType);

                    //other.node.parent.getComponent(cc.Animation).play("Harvest");
                    this.harvestAnim();
                    
                    GameController.instance.bunnicontroller.jumped = false;
                    // GameController.instance.harvestController.isHarvesting = false;
                    // GameController.instance.bunnicontroller.canHarvest = false;
                }
            }
        }
    }

    harvestAnim(){

        cc.tween(this.node).sequence(cc.spawn(
            cc.jumpBy(0.5,cc.v2(150,0),250,2),
            cc.callFunc(()=>{
                GameController.instance.harvestController.isHarvesting = false;
                GameController.instance.bunnicontroller.canHarvest = false;
    
                cc.audioEngine.playEffect(this.harvestSFX,false);
                
                this.node.getComponentInChildren(cc.Animation).play();
            })
            ),cc.fadeOut(.5), cc.callFunc(()=>{
                this.node.destroy();
        })).start();
    }


    // update (dt) {}
}
