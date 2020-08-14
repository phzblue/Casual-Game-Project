import GameController from "./GameController";
import CatController from "./CatController";

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
export default class MouseController extends cc.Component {

    @property(cc.Node)
    cat:cc.Node = null;

    startDragPos:cc.Vec2 = null;
    endDragPos:cc.Vec2 = null;
    
    // LIFE-CYCLE CALLBACKS:

    // onLoad () {}

    start () {
        this.node.on(cc.Node.EventType.TOUCH_START, (event)=>{
            this.startDragPos = event.getLocation();
        },this.node);

        this.node.on(cc.Node.EventType.TOUCH_END, (event)=>{
            this.endDragPos = event.getLocation();
            this.calculateDragDistanceOrTap();
        },this.node)
    }

    calculateDragDistanceOrTap(){
        if(!GameController.instance.isGameOver && !GameController.instance.isStunned){
            let direction = this.endDragPos.sub(this.startDragPos);
            if(Math.abs(direction.x)>50){
                this.cat.getComponent(CatController).checkCatMovement(Math.sign(direction.x));
            }else{
                GameController.instance.tapHuman();
            }
        }
    }

    // update (dt) {}
}
