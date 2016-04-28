using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;


namespace ElevatorSystem
{
     class ElevatorCtrl:INotifyPropertyChanged
    {
        Elevator[] elevator;
        ListWrapper[] taskCacheUp;
        ListWrapper[] taskCacheDown;
        int size;
         public event PropertyChangedEventHandler PropertyChanged;
        //构造函数
        public ElevatorCtrl(int ElevatorNum)
        {
            elevator = new Elevator[ElevatorNum];
            for (int i = 0; i < ElevatorNum;i++ ) elevator[i]=new Elevator();
                size = ElevatorNum;
            taskCacheUp = new ListWrapper[19];
            taskCacheDown = new ListWrapper[19];
            for (int i = 0; i < 19; i++)
            {
                taskCacheUp[i] = new ListWrapper();
                taskCacheDown[i] = new ListWrapper();
            }
        }

        //基础接口
        public int Size
        {
            get { return size; }
        }

        public Elevator[] Elevator
        {
            get{return elevator;}
        }

        public ListWrapper[] TaskCacheUp
        {
            get { return taskCacheUp; }
        }
        public ListWrapper[] TaskCacheDown
        {
            get { return taskCacheDown; }
        }
        public void stop(int eleNum)//停止某个电梯
        {
            elevator[eleNum].State = STATE.STOP;
        }

        //任务添加和完成
        public void addTaskInside(int floor, int elevatorNum)
        {
            if (elevator[elevatorNum].State == STATE.STOP) return;
           elevator[elevatorNum].addTaskInside(floor);
        }
        public void addTaskOutside(int floor, bool up)
        {
            if (up) taskCacheUp[floor - 1].task = true;
            else taskCacheDown[floor - 2].task = true;
        }
        public void taskFinish(int floor, STATE state)
        {
            if (state == STATE.UP)
                taskCacheUp[floor - 1].task = false;
            else taskCacheDown[floor - 2].task = false;
        }
        //任务分派
        public void dispatchTask(int floor, STATE state)
        {
            int[] prior = new int[size];
            int best=0;
            for(int i=0;i<size;i++) //计算最大可能运动的楼层差
            {
                prior[i] = 100;
                if (elevator[i].State == STATE.STOP) continue;
                if(elevator[i].Floor==floor &&(elevator[i].State==state || elevator[i].State==STATE.PAUSE))
                {
                    taskFinish(floor, state);
                }
                else if(state==elevator[i].State)  //同向
                {
                    if ((state == STATE.UP) == (floor >= elevator[i].Floor))    //在前面
                        prior[i] = System.Math.Abs(floor - elevator[i].Floor);
                    else
                        prior[i] = 38 - System.Math.Abs(floor - elevator[i].Floor);
                }
                else
                {           //异向
                    if (state == STATE.UP) prior[i] = floor + elevator[i].Floor - 2;
                    else prior[i] = 40 - floor - elevator[i].Floor;
                }
            }
            //挑选经过楼层差最小的电梯并分派任务
            for (int i = 0; i < size; i++) { if (prior[i] < prior[best]) best = i; }
            elevator[best].addTaskOutside(floor, state);
        }
        
         //总运行
        public void run() 
        {
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {    //完成外部任务
                    STATE state = Elevator[i].State;
                    int floor = Elevator[i].Floor;
                    if (state == STATE.STOP) continue;
                    if (floor<=19 && taskCacheUp[floor - 1].task)
                    {
                        if (state == STATE.UP || state == STATE.PAUSE) taskFinish(floor, STATE.UP);
                    }
                    if (floor == 20) taskFinish(20, STATE.DOWN);
                    if (floor >= 2 && taskCacheDown[floor - 2].task)
                    {
                        if (state == STATE.DOWN || state == STATE.PAUSE) taskFinish(floor, STATE.DOWN); 
                    }
                    if (floor == 1) taskFinish(1, STATE.UP);
                        //清空已分派任务
                    Elevator[i].clear();
                }
                for (int j = 0; j < 19; j++)//重新分派任务
                {
                    if (taskCacheUp[j].task) dispatchTask(j + 1, STATE.UP);
                    if (taskCacheDown[j].task) dispatchTask(j + 2, STATE.DOWN);
                }
                Thread.Sleep(800);
                for (int i = 0; i < 5; i++)
                {
                    if (elevator[i].State == STATE.STOP) continue;
                    elevator[i].run();
                }
            }
        }
    }
}
