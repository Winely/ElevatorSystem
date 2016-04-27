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
        bool[,] taskCache;
        int size;
         public event PropertyChangedEventHandler PropertyChanged;
        //构造函数
        public ElevatorCtrl(int ElevatorNum)
        {
            elevator = new Elevator[ElevatorNum];
            for (int i = 0; i < ElevatorNum;i++ ) elevator[i]=new Elevator();
                size = ElevatorNum;
            taskCache = new bool[2,20];
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

         public bool this[int i, int j]
        {
            get { return taskCache[i,j]; }
            set
            {
                if (this.PropertyChanged != null){
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TaskCache"));
            }
            }
        }
        public void stop()//全部停止
        {
            for (int i = 0; i < size; i++) elevator[i].stop();
        }

        //任务添加和完成
        public void addTaskInside(int floor, int elevatorNum)
        {
           elevator[elevatorNum].addTaskInside(floor);
        }
        public void addTaskOutside(int floor, bool up)
        {
            taskCache[up ? 0 : 1, floor]=true;
        }
        public void taskFinish(int floor, STATE state, int eleNum)
        {
            if(taskCache[state==STATE.UP? 0 : 1, floor - 1])
            {
                taskCache[state == STATE.UP ? 0 : 1, floor - 1] = false;
            }
        }
        //任务分派
        public void dispatchTask(int floor, STATE state)
        {
            int[] prior = new int[size];
            int best=0;
            for(int i=0;i<size;i++) //计算最大可能运动的楼层差
            {
                if(elevator[i].State==STATE.PAUSE) //电梯没动
                    prior[i]=System.Math.Abs(floor-elevator[i].Floor);
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
                {    //取消外部任务
                    STATE state = Elevator[i].State;
                    int floor = Elevator[i].Floor;
                    if (state == STATE.PAUSE)
                    {
                        taskFinish(floor, STATE.UP, i);
                        taskFinish(floor, STATE.DOWN, i);
                    }
                    else taskFinish(floor, state, i);
                    //清空已分派任务
                    Elevator[i].clear();
                }
                for (int i = 0; i < 2; i++)//重新分派任务
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (taskCache[i, j]) dispatchTask(j + 1, (STATE)i);
                    }
                }
                for (int i = 0; i < 5; i++) Elevator[i].run();
                Thread.Sleep(800);
            }
        }
    }
}
