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
export default class NewClass extends cc.Component {

    startDragPos:cc.Vec2 = null;
    currentDragPos:cc.Vec2 = null;
    endDragPos:cc.Vec2 = null;

    start () {
        this.node.on(cc.Node.EventType.TOUCH_START, (event)=>{
            this.startDragPos = event.getLocation();
        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_MOVE, (event)=>{
            this.currentDragPos = event.getLocation();
            this.calculateMidDrag();
        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_END, (event)=>{
            this.endDragPos = event.getLocation();
            this.calculateDragDistanceOrTap();
        },this.node)
    }

    calculateDragDistanceOrTap(){
        if(!GameController.instance.isGameOver){
            let direction = this.endDragPos.sub(this.startDragPos);
            GameController.instance.swipe(direction.x);            
        }
    }

    calculateMidDrag(){
        if(!GameController.instance.isGameOver){
            let direction = this.startDragPos.sub(this.currentDragPos);
            GameController.instance.moveCard(direction);            
        }
    }

    // update (dt) {}
}
