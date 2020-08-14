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
export default class JumpController extends cc.Component {

    jumpOn:boolean = false;

    bunniObj:cc.Node = null;
    bunniDefaultYPos:number = 0;
    jumpAnim:boolean = false;

    @property({
        type: cc.AudioClip
    })
    jumpSFX:cc.AudioClip = null;

    onLoad(){
        this.bunniObj = GameController.instance.bunni;
    }

    start () {
        this.bunniDefaultYPos = this.bunniObj.position.y;

        this.node.on(cc.Node.EventType.TOUCH_START, ()=>{
            this.jumpOn = true;
        });

        this.node.on(cc.Node.EventType.TOUCH_CANCEL, ()=>{
            this.jumpOn = false;
        });

        this.node.on(cc.Node.EventType.TOUCH_END, ()=>{
            this.jumpOn = false;
        });
    }
    
    update (dt) {

        if(this.jumpOn){
            if(this.bunniObj.position.y < this.bunniDefaultYPos+800 && !GameController.instance.bunnicontroller.jumped){

                if(!this.jumpAnim){
                    this.jumpAnim = true;
                    this.bunniObj.getComponent(cc.Animation).play("Jump");
                    cc.audioEngine.playEffect(this.jumpSFX,false);
                }

                if(this.bunniObj.position.y < this.bunniDefaultYPos+800){
                    this.bunniObj.setPosition(this.bunniObj.position.x, this.bunniObj.position.y+10)

                }
            }else if(this.bunniObj.position.y >= (this.bunniDefaultYPos+800) || GameController.instance.bunnicontroller.jumped){


                GameController.instance.bunnicontroller.jumped = true;
                GameController.instance.bunnicontroller.canHarvest = true;

                if(this.jumpAnim){
                    this.jumpAnim = false;
                    this.bunniObj.getComponent(cc.Animation).play("Idle");
                }

                if(this.bunniObj.position.y > this.bunniDefaultYPos){
                    this.bunniObj.setPosition(this.bunniObj.position.x, this.bunniObj.position.y-10)

                }else if(this.bunniObj.position.y < this.bunniDefaultYPos && this.bunniObj.position.y != this.bunniDefaultYPos){

                    this.bunniObj.setPosition(this.bunniObj.position.x, this.bunniDefaultYPos)
                }
            }
        }else if(!this.jumpOn){

            if(this.bunniObj.getComponent(cc.Animation).currentClip != null &&
            this.bunniObj.getComponent(cc.Animation).currentClip.name != "Idle"){
                this.bunniObj.getComponent(cc.Animation).play("Idle");
                this.jumpAnim = false;
            }

            if(this.bunniObj.position.y > this.bunniDefaultYPos){
                this.bunniObj.setPosition(this.bunniObj.position.x, this.bunniObj.position.y-10)
            }

            if(this.bunniObj.position.y > this.bunniDefaultYPos){
                GameController.instance.bunnicontroller.jumped = true;
                GameController.instance.bunnicontroller.canHarvest = true;

            }else if(GameController.instance.bunnicontroller.jumped){
                GameController.instance.bunnicontroller.jumped = false;
                GameController.instance.bunnicontroller.canHarvest = false;
            }
        }
    }
}
