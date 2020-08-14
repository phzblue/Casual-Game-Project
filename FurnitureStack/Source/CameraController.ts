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
export default class CameraController extends cc.Component {

    @property
    isMoveCameraUp : boolean = false;

    isMoving:boolean = false;
    
    start () {
        this.node.on(cc.Node.EventType.TOUCH_START, (event)=>{
            this.isMoving = true;
        }, this.node);


        this.node.on(cc.Node.EventType.TOUCH_END, (event)=>{
            this.isMoving = false;
        }, this.node);
    }

    update () {
        if(this.isMoving && !GameController.instance.isGameOver){
            if(this.isMoveCameraUp){
                GameController.instance.moveCameraUp();
            }else{
                GameController.instance.moveCameraDown();
            }
        }
    }
}
