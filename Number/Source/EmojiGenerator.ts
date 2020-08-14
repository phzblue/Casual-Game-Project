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
export default class EmojiGenerator extends cc.Component {

    @property(cc.Prefab)
    positiveEmojis:cc.Prefab[] = [];

    @property(cc.Prefab)
    negativeEmojis:cc.Prefab[] = [];

    @property(cc.Node)
    leftContainer:cc.Node = null;

    @property(cc.Node)
    rightContainer:cc.Node = null;

    spawnEmoji(direction:any, isWin:boolean){

        let newEmoji = cc.instantiate(isWin?this.positiveEmojis[this.getRandom(this.positiveEmojis.length-1)]:
            this.negativeEmojis[this.getRandom(this.negativeEmojis.length-1)])

        if(direction < 0){
            newEmoji.setParent(this.leftContainer);
        }else if(direction > 0){
            newEmoji.setParent(this.rightContainer);
        }

        //newEmoji.setPosition(newEmoji.position.sub(cc.v2(this.getRandom(100,-100),0)));

        let shakeLeftRight = cc.sequence(cc.moveBy(.5,cc.v2(100,0)),cc.moveBy(.5,cc.v2(-100,0))).repeatForever();
        let moveUp = cc.moveBy(2.5,cc.v2(0,2000));
        let move = cc.moveBy(2.5, cc.v2(500*-direction,0));
        let fadeout = cc.fadeOut(0.2);
        let destroy = cc.callFunc(()=>{ newEmoji.destroy(); })

        newEmoji.runAction(cc.sequence(cc.spawn(moveUp,move,shakeLeftRight), fadeout, destroy));
    }

    getRandom(max:number, min:number = 0){
        return Math.floor(Math.random() * (max - min) + min); 
    }

    // update (dt) {}
}
