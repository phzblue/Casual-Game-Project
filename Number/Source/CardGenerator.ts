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
export default class CardGenerator extends cc.Component {

    @property(cc.SpriteFrame)
    defaultCard:cc.SpriteFrame = null;

    @property(cc.Prefab)
    cardPrefab:cc.Prefab = null;

    @property(cc.Node)
    cardBack:cc.Node = null;

    max:number = 0;
    min:number = 0;

    atlasArray:cc.SpriteAtlas[] = [];

    cardSymbolMap:Map<number,number> = new Map();

    onLoad(){
        let self = this;

        cc.loader.loadResDir("Cards", cc.SpriteAtlas, function (err, atlas) {
            self.atlasArray = atlas;
            console.log("test")
        });
    }

    start(){
        this.reset();
    }

    reset(){
        this.cardSymbolMap.clear();

        this.max = this.getRandom(1000, 200);
        this.min = this.max-200;

        let symbolNum = 0;
        let cardNum = 0;
        this.cardSymbolMap.set(0,0);

        for(let i = this.min; i<this.max; i++){
            cardNum++;
            this.cardSymbolMap.set(i,symbolNum);
            if(cardNum % 4 == 0){
                symbolNum++;
            }
        }
    }
    
    generateCard(firstCard:boolean = false){
        let newCard = cc.instantiate(this.cardPrefab);
        let num = this.getRandom(this.max,this.min);

        if(GameController.instance.currentNumber == num){
            num += 1
        }

        if(firstCard){
            num = 0;
            newCard.getComponentInChildren(cc.Sprite).spriteFrame = this.defaultCard;
        }else{
            this.atlasArray.forEach((value)=>{
                if(value.getSpriteFrame("Card"+this.cardSymbolMap.get(num))!=null){
                    newCard.getComponentInChildren(cc.Sprite).spriteFrame = value.getSpriteFrame("Card"+this.cardSymbolMap.get(num));
                    return;
                }
            });
        }

        newCard.getComponentInChildren(cc.Label).string = num.toString();
        newCard.setParent(this.cardBack);

        GameController.instance.cardNum++;
        GameController.instance.cardNumText.getComponent(cc.Label).string = GameController.instance.cardNum.toString();

        return num;
    }

    getRandom(max:number, min:number = 0){
        return Math.floor(Math.random() * (max - min) + min); 
    }

}
