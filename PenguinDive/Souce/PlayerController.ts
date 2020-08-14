import GameController from "./GameController";
import Scroller from "./Scroller"
import TouchController from "./TouchController";

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
export default class PlayerController extends cc.Component {

    static instance:PlayerController = null;

    speed:number = 10;
    direction:number = 0;
    canBounce:boolean = true;

    @property({
        type: cc.AudioClip
    })
    fishSFX:cc.AudioClip = null;

    @property({
        type: cc.AudioClip
    })
    jellySFX:cc.AudioClip = null;

    @property(cc.Node)
    bubbleObj:cc.Node = null;

    @property(cc.Node)
    diveLine:cc.Node = null;

    @property(cc.Node)
    waterLine:cc.Node = null;

    @property(cc.Node)
    playerContainer:cc.Node = null;

    // @property(Scroller)
    // parrallaxScroll:Scroller = null;
    // @property(Scroller)
    // jellyScroll:Scroller = null;

    onLoad(){
        PlayerController.instance = this;
    }

    startDiving(){

        cc.tween(this.playerContainer)
        .then(
            cc.spawn(
                cc.moveBy(0.5,cc.v2(0,-550)),
                cc.callFunc(()=>{
                    this.bubbleObj.getComponent(cc.Animation).play();
                    this.diveLine.getComponent(cc.Animation).play();

                    cc.director.getPhysicsManager().enabled = true;

                })
            )
        ).by(0.5, {position:cc.v2(0,50)})
        .start();        

        cc.tween(this.waterLine).then(
            cc.moveBy(0.5,cc.v2(0,250)),
        ).start();
    }

    moveCamera(){
        let cameJumpDir = 0
        if(this.direction == 0){
            cameJumpDir = 1;
        }else{
            cameJumpDir = this.direction;
        }

        //this.parrallaxScroll.moveScroller(cameJumpDir);
        //this.jellyScroll.moveScroller(cameJumpDir);

        let jumpRange = 150*cameJumpDir;

        TouchController.instance.canMove = false;

        if(this.node.parent.x+jumpRange < (this.node.parent.parent.getContentSize().width/2) && this.node.parent.x+jumpRange > -(this.node.parent.parent.getContentSize().width/2)){
            cc.tween(this.node.parent).then(cc.jumpBy(1,cc.v2(jumpRange,0),300,1)).call(()=>{
                this.canBounce=true
                TouchController.instance.canMove = true;
            }).start();
        }else{
            cc.tween(this.node.parent).then(cc.jumpBy(1,cc.v2(-jumpRange,0),300,1)).call(()=>{
                this.canBounce=true;
                TouchController.instance.canMove = true;
            }).start();
        }
        
    }

    checkCameraArea(playerPos:number){
        return playerPos < (this.node.parent.x + cc.view.getCanvasSize().width/2) &&
        playerPos > (this.node.parent.x - cc.view.getCanvasSize().width/2);
    }

    resetWaterLine(){
        cc.tween(this.waterLine).then(
            cc.moveBy(0.1,cc.v2(0,-250)),
        ).start();
    }

    stopAnim(){
        this.bubbleObj.getComponent(cc.Animation).stop();
        this.diveLine.getComponent(cc.Animation).stop();
    }

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(!GameController.instance.isGameOver){
            if(other.name.includes("coin")){
                GameController.instance.scoreKeeper.increaseScore();
                other.node.getComponent(cc.Animation).play();

                cc.audioEngine.play(this.fishSFX,false,1);
                cc.tween(other.node).call(()=>{
                    other.node.getComponent(cc.BoxCollider).enabled = false;
                }).delay(0.2).call(()=>{
                    other.node.destroy();
                })
            }else if(other.name.includes("obstacle") && this.canBounce){
                this.canBounce=false;

                GameController.instance.scoreKeeper.currentCombo = 0;
                GameController.instance.scoreKeeper.updateText();
                other.node.getComponent(cc.Animation).play();
                cc.audioEngine.play(this.jellySFX,false,1);

                this.moveCamera();
            }
        }
    }
}
