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

    currentAction:cc.Action = null;
    cardPos:cc.Vec2 = null;

    isLeft:boolean = false;
    isRight:boolean = false;

    start(){
        this.cardPos = this.node.position;
    }

    shiftCardLeft(){
        if(this.isRight || !this.isLeft){
            this.isLeft = true;
            this.isRight = false;
            if(this.currentAction == null || (this.currentAction != null && this.currentAction.isDone)){
                this.currentAction = this.node.runAction(cc.sequence(cc.moveTo(0.1,this.cardPos), cc.spawn(
                    cc.rotateTo(.2,10),
                    cc.moveBy(0.2,cc.v2(this.cardPos.x+50,0))
                )))
            }
        }
    }

    resetCardPos(){
        this.node.angle = 0;
    }

    shiftCardRight(){
        if(!this.isRight || this.isLeft){
            this.isLeft = false;
            this.isRight = true;

            if(this.currentAction == null || (this.currentAction != null && this.currentAction.isDone)){
                this.currentAction = this.node.runAction(cc.sequence(cc.moveTo(0.1,this.cardPos), cc.spawn(
                    cc.rotateTo(.2,-10),
                    cc.moveBy(0.2,cc.v2(this.cardPos.x-50,0))
                )))
            }
        }
        
    }
}
