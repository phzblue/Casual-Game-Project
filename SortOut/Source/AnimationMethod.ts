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
export default class AnimationMethod extends cc.Component {

    shake(direction){
        //UP,DOWN,LEFT,RIGHT

        let loc:cc.Vec2;
        let size = 2;
        switch(direction){
            case 0:
                loc = new cc.Vec2(0,size);
                break;
            case 1:
                loc = new cc.Vec2(0,-size);
                break;
            case 2:
                loc = new cc.Vec2(-size,0);
                break;
            case 3:
                loc = new cc.Vec2(size,0);
                break;
        }

        let move = cc.moveBy(.3,loc);
        let moveBack = cc.moveBy(.3,loc.mul(-1));

        this.node.runAction(cc.sequence(move,moveBack));
    }

    toggleVibrate(isStop:boolean){

        if(isStop){
            this.node.stopAllActions();
            this.node.angle = 0;
        }else{
            this.node.runAction(cc.repeatForever(cc.sequence(cc.rotateBy(.2,5) , cc.rotateTo(.2,0), cc.rotateBy(.2,-5), cc.rotateTo(.2,0))))
        }
        
    }

    toggleFadeAnimation(isStop:boolean){
        if(isStop){
            this.node.stopAllActions();
            this.node.runAction(cc.fadeOut(0));
        }else{
            this.node.runAction(cc.repeatForever(cc.sequence(cc.fadeIn(.2),cc.fadeOut(.2))));
        }

    }
}
