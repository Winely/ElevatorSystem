using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace ElevatorSystem
{
    enum STATE { UP, DOWN, PAUSE, EXCEPTION, STOP };//电梯状态 亦可用于外部指令
    class Elevator:INotifyPropertyChanged
    {
        STATE state=STATE.PAUSE;
        int floor=1;
        ListWrapper[] taskInside = new ListWrapper[20];//任务列表
        bool[,] taskOutside = new bool[2, 20];
        public event PropertyChangedEventHandler PropertyChanged;
        //Property
        public Elevator() 
        {
            for (int i = 0; i < 20; i++)
            {
                taskInside[i] = new ListWrapper();
            }
        }
        public STATE State
        {
            get { return state; }
            set { state = value;
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("State"));
            }
        }
        public int Floor
        {
            get { return floor; }
            set { floor = value;
                if (this.PropertyChanged != null)
                { 
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Floor")); 
                    //检查取消事件
                    taskFinish(value);//完成相应内部任务
                }
            }
        }

        public ListWrapper[] TaskInside
        {
            get { return taskInside; }
        }

        public void run()
        {
            if (state == STATE.EXCEPTION) return;
            if (state == STATE.STOP) state = STATE.PAUSE;
            switch(state)
            {
                case STATE.PAUSE:
                case STATE.UP:
                    if (taskInside[floor - 1].task) taskInside[floor - 1].task = false;

                    for(int i=floor;i<20;i++)
                    { 
                        if (taskInside[i].task || taskOutside[(int)STATE.UP, i]||taskOutside[(int)STATE.DOWN, i])
                        {
                            up(); return;
                        }
                        pause();
                    }
                    for(int i=0;i<floor-1;i++)
                    {
                        if(taskInside[i].task||taskOutside[(int)STATE.DOWN, i]||taskOutside[(int)STATE.UP, i])
                        {
                            down(); return;
                        }
                    }
                    break;
                case STATE.DOWN:
                    for (int i = 0; i < floor - 1; i++)
                    {
                        if (taskInside[i].task || taskOutside[(int)STATE.DOWN, i] || taskOutside[(int)STATE.UP, i])
                        {
                            down(); return;
                        }
                        pause();
                    }
                    for (int i = floor; i < 20; i++)
                    {
                        if (taskInside[i].task || taskOutside[(int)STATE.UP, i] || taskOutside[(int)STATE.DOWN, i])
                        {
                            up(); return;
                        }
                    }
                    break;
            }

        }

        //电梯运动·分解动作
        public void up()        //电梯上行
        {
            Floor++;
            state=STATE.UP;
        }

        public void down()      //电梯下行
        {
            Floor--;
            state=STATE.DOWN;
        }
        public void pause()      //电梯停下（开门）
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
            taskInside[task - 1].task = true;
        }
        public void addTaskOutside(int task, STATE direct)   //电梯外按键
        {
            taskOutside[(int)direct, task-1]=true;
        }
        
        //完成任务
        public void taskFinish(int task)
        {
            if (taskInside[task - 1].task)
                taskInside[task - 1].task = false;
        }
       
    }

    class ListWrapper:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool task_value = false;
        public bool task
        {
            get { return task_value; }
            set
            {
                task_value = value;
            if (this.PropertyChanged != null){
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("task"));
            }
            }
        }
    }


}