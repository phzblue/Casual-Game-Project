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
export default class Util extends cc.Component {
    
    static secondToMinute(number:number){
        let min, sec;

        min = Math.floor(number / 60);
        sec = ("0" + Math.floor(number>=60 ? number-60 : number)).slice(-2)

        return min+":"+sec;
    }

    static getRandom(max:number, min:number = 0){
        return Math.floor(Math.random() * (max - min) + min); 
    }

    static getRandomFloat(max:number, min:number = 0){
        return Math.random() * (max - min) + min; 
    }

    static getRandomCollectionObject(collection) {
        let index = Math.floor(Math.random() * collection.size);
        let cntr = 0;
        for (let key of collection.keys()) {
            if (cntr++ === index) {
                return key;
            }
        }
    }

    static getRandomArrayObject(collection){
        let index = Math.floor(Math.random() * collection.length);
        let cntr = 0;

        for (var obj of collection) {
            if (cntr++ === index) {
                return obj;
            }
        }
    }

    static stringToVector(coordinate:string){
        return new cc.Vec2(Number(coordinate.split(",")[0]),Number(coordinate.split(",")[1]));
    }

    static vectorToString(coordinate:cc.Vec2){
        return coordinate.x+","+coordinate.y;
    }

    static shuffleArray(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    }
}
