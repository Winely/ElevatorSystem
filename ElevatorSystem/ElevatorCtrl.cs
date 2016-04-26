using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ElevatorSystem
{
     class ElevatorCtrl
    {
        Elevator[] elevator;
        bool[,] taskCache;
        int size;

        //构造函数
        public ElevatorCtrl(int ElevatorNum)
        {
            elevator = new Elevator[ElevatorNum];
            size = ElevatorNum;
            taskCache = new bool[2,20];
        }

        //基础接口
        public int getSize() { return size; }
        public void run() //全部启动
        {
            for (int i = 0; i < size; i++) elevator[i].start();
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
            taskCache[up ? 0 : 1, floor-1]=true;
        }
        public void taskFinish(int floor, bool up)
        {
            taskCache[up ? 0 : 1, floor - 1] = false;
        }
        //任务分派
        public void dispatchTask(int floor, STATE state)
        {
            int[] prior = new int[size];
            int best=0;
            for(int i=0;i<size;i++) //计算最大可能运动的楼层差
            {
                if(elevator[i].getState()==STATE.PAUSE) //电梯没动
                    prior[i]=System.Math.Abs(floor-elevator[i].getFloor());
                else if(state==elevator[i].getState())  //同向
                {
                    if ((state == STATE.UP) == (floor >= elevator[i].getFloor()))    //在前面
                        prior[i] = System.Math.Abs(floor - elevator[i].getFloor());
                    else
                        prior[i] = 38 - System.Math.Abs(floor - elevator[i].getFloor());
                }
                else
                {           //异向
                    if (state == STATE.UP) prior[i] = floor + elevator[i].getFloor() - 2;
                    else prior[i] = 40 - floor - elevator[i].getFloor();
                }
            }
            //挑选经过楼层差最小的电梯并分派任务
            for (int i = 0; i < size; i++) { if (prior[i] < prior[best]) best = i; }
            elevator[best].addTaskOutside(floor, state);
        }

        //总更新
        public void update()
        {
            for (int i = 0; i < size; i++)  elevator[i].clear();  //清除所有已分派外部任务

            for(int i=0;i<2;i++)        //重新分派
            {
                for(int j=0;j<20;j++)
               {
                   if (taskCache[i,j]) dispatchTask(j + 1, (STATE)i);
                }
            }
        }
    }
}
