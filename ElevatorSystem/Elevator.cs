using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem
{
    enum STATE { UP, DOWN, PAUSE, EXCEPTION, STOP };//电梯状态 亦可用于外部指令
    class Elevator
    {
        STATE state=STATE.PAUSE;
        int floor=1;
        bool[] taskInside = new bool[20];//任务列表
        bool[,] taskOutside = new bool[2, 20];
        //List<int>[] stateCache;
        public Elevator() { }
        public STATE State
        {
            get { return state; }
            set { state = value; }
        }
        public int Floor
        {
            get { return floor; }
            set { floor = value; }
        }
        public void run()
        {
            if (state == STATE.STOP) state = STATE.PAUSE;
        }

        //电梯运动·分解动作
        public void up()        //电梯上行
        {
            floor++;
            state=STATE.UP;
        }

        public void down()      //电梯下行
        {
            floor--;
            state=STATE.DOWN;
        }
        public void pause()      //电梯停下（开门）
        {
            state = STATE.PAUSE;
        }
        public void start()
        {
            state = STATE.PAUSE;
        }
        public void stop()
        {
            state = STATE.STOP;
        }
        
        public void clear()             //清除所有外部任务
        {
            for(int i=0;i<2;i++){
                for (int j = 0; j < 20; j++)
                    taskOutside[i, j] = false;
            }
        }

        //添加任务
        public void addTaskInside(int task)   //电梯内按键
        {
            taskInside[task - 1] = true;
        }
        public void addTaskOutside(int task, STATE direct)   //电梯外按键
        {
            taskOutside[(int)direct, task-1]=true;
        }
       
    }
}
