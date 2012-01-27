using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    public class DebugDataMonitor
    {
        
        public Queue<int> dataPoints;
        public int intervalTime = 0;
        public int currentBatch = 0;
        public int maxData = 0;

        
        public DebugDataMonitor()
        {
            dataPoints = new Queue<int>(60);
        }

        public void Update(int gameTime)
        {
            //intervalTime += gameTime;
            //if (intervalTime > 1000)
            {
                //intervalTime -= 1000;
                dataPoints.Enqueue(currentBatch);
                if(dataPoints.Count > 100)
                    dataPoints.Dequeue();
                currentBatch = 0;
            }
        }

        public void AddData(int dataSet)
        {
            currentBatch += dataSet;
            if (dataSet > maxData)
                maxData = dataSet;
        }

        public int Average()
        {
            if (dataPoints.Count == 0)
                return 0;
            return (int)dataPoints.Average();
        }
    }
}
