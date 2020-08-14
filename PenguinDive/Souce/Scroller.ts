// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

const {ccclass, property} = cc._decorator;

@ccclass
export default class NewClass extends cc.Component {

    speed:number = 100;

    rigthEdge:number = 0;
    leftEdge:number = 0;

    @property
    isJelly:boolean = false;

    start(){
        if(this.isJelly){
            this.rigthEdge = 800;
            this.leftEdge = -800;

            this.speed = 50;
        }else{
            this.rigthEdge = this.node.getContentSize().width / 2 - 700;
            this.leftEdge = -(this.node.getContentSize().width / 2) + 700;
        }
        
    }

    moveScroller(direction:number){

        let x = this.speed;
        x *= direction;

        if(this.isJelly){
            if(this.node.x < this.leftEdge){
                cc.tween(this.node).then(cc.fadeOut(0.5)).to(0,{position:cc.v2(100,0)}).then(cc.fadeIn(0.2)).by(0.5,{position: cc.v2(-x,0)}).start();
            }else if(this.node.x > this.rigthEdge){
                cc.tween(this.node).then(cc.fadeOut(0.5)).to(0,{position:cc.v2(-100,0)}).then(cc.fadeIn(0.2)).by(0.5,{position: cc.v2(-x,0)}).start();
            }else{
                cc.tween(this.node).by(0.5,{position: cc.v2(-x,0)}).start();
            }
        }else{
            if(this.node.x < this.leftEdge){
                cc.tween(this.node).to(0,{position:cc.v2(3500,0)}).then(cc.fadeIn(0.2)).by(0.5,{position: cc.v2(-x,0)}).start();
            }else if(this.node.x > this.rigthEdge){
                cc.tween(this.node).to(0,{position:cc.v2(-3500,0)}).then(cc.fadeIn(0.2)).by(0.5,{position: cc.v2(-x,0)}).start();
            }else{
                cc.tween(this.node).by(0.5,{position: cc.v2(-x,0)}).start();
            }
        }

    }

    // update (dt) {}
}
