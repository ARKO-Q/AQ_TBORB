#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

/******************************************************************************
File	         :   AQDCFINTORB.cs
Author			 :   ARKO Quantum S.R.L.
Created          :   04/03/2025
Updated			 :	 06/03/2025
------------------------------------------------------------------------------
Email            :   contacto@arkoquantum.com
Website          :   arkoquantum.com

Copyright   ARKO Quantum S.R.L. 2025. All rights reserved.
*****************************************************************************/

namespace NinjaTrader.NinjaScript.Indicators
{
    public class AQDFCINTBORB : Indicator
	{
        #region Variables

        private double openingRangeHigh = double.MinValue;
        private double openingRangeLow = double.MaxValue;
        private bool rangeSet = false;

        #endregion

        #region Settings
        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Time Based Opening Range Breakout High and Low Lines.";
				Name										= "AQDFCINTBORB";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				StartTime									= DateTime.Parse("08:31", System.Globalization.CultureInfo.InvariantCulture);
				EndTime										= DateTime.Parse("08:35", System.Globalization.CultureInfo.InvariantCulture);

				AddPlot(Brushes.White, "ORB High");
				AddPlot(Brushes.Blue, "ORB Low");


			}
			else if (State == State.Configure)
			{
			}
		}
        #endregion

        #region Logic
        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1) return;

            if (Bars.IsFirstBarOfSession)
            {
                openingRangeHigh = double.MinValue;
                openingRangeLow = double.MaxValue;
                rangeSet = false;
            }

            TimeSpan barTime = Times[0][0].TimeOfDay;
            TimeSpan startTimeSpan = StartTime.TimeOfDay;
            TimeSpan endTimeSpan = EndTime.TimeOfDay;

            if (barTime >= startTimeSpan && barTime <= endTimeSpan)
            {
                openingRangeHigh = Math.Max(openingRangeHigh, High[0]);
                openingRangeLow = Math.Min(openingRangeLow, Low[0]);
                rangeSet = true;
            }

            if (rangeSet)
            {
                Values[0][0] = openingRangeHigh;
                Values[1][0] = openingRangeLow;
            }
        }
        #endregion

        #region Properties
        [NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="StartTime", Order=1, GroupName="Parameters")]
		public DateTime StartTime
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="EndTime", Order=2, GroupName="Parameters")]
		public DateTime EndTime
		{ get; set; }
        public bool IsFirstBarOfSession { get; private set; }
        #endregion

    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AQDFCINTBORB[] cacheAQDFCINTBORB;
		public AQDFCINTBORB AQDFCINTBORB(DateTime startTime, DateTime endTime)
		{
			return AQDFCINTBORB(Input, startTime, endTime);
		}

		public AQDFCINTBORB AQDFCINTBORB(ISeries<double> input, DateTime startTime, DateTime endTime)
		{
			if (cacheAQDFCINTBORB != null)
				for (int idx = 0; idx < cacheAQDFCINTBORB.Length; idx++)
					if (cacheAQDFCINTBORB[idx] != null && cacheAQDFCINTBORB[idx].StartTime == startTime && cacheAQDFCINTBORB[idx].EndTime == endTime && cacheAQDFCINTBORB[idx].EqualsInput(input))
						return cacheAQDFCINTBORB[idx];
			return CacheIndicator<AQDFCINTBORB>(new AQDFCINTBORB(){ StartTime = startTime, EndTime = endTime }, input, ref cacheAQDFCINTBORB);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AQDFCINTBORB AQDFCINTBORB(DateTime startTime, DateTime endTime)
		{
			return indicator.AQDFCINTBORB(Input, startTime, endTime);
		}

		public Indicators.AQDFCINTBORB AQDFCINTBORB(ISeries<double> input , DateTime startTime, DateTime endTime)
		{
			return indicator.AQDFCINTBORB(input, startTime, endTime);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AQDFCINTBORB AQDFCINTBORB(DateTime startTime, DateTime endTime)
		{
			return indicator.AQDFCINTBORB(Input, startTime, endTime);
		}

		public Indicators.AQDFCINTBORB AQDFCINTBORB(ISeries<double> input , DateTime startTime, DateTime endTime)
		{
			return indicator.AQDFCINTBORB(input, startTime, endTime);
		}
	}
}

#endregion
