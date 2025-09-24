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
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
using System.Windows.Forms.VisualStyles;
using System.Runtime.Remoting.Lifetime;
using NinjaTrader.Gui.NinjaScript.StrategyAnalyzer;
#endregion

/***************************************************************************************
  AAAAA   RRRRR    K   K   OOO      QQQQQ   U   U   AAAAA   N   N  TTTTT  U   U  M   M  
 A     A  R    R   K  K   O   O    Q    Q   U   U  A     A  NN  N    T    U   U  MM MM  
 AAAAAAA  RRRRR    KKK    O   O    Q  Q Q   U   U  AAAAAAA  N N N    T    U   U  M M M  
 A     A  R   R    K  K   O   O    Q   QQ   U   U  A     A  N  NN    T    U   U  M   M  
 A     A  R    R   K   K   OOO     QQQQQ Q  UUUU   A     A  N   N    T     UUUU  M   M  
 **************************************************************************************/

/******************************************************************************
 * File	            :   AQDFCSITBORB.cs                                                
 * Author			:   ARKO Quantum S.R.L.                                              
 * Created          :   04/03/2025                                                         
 * Updated			:	08/04/2025                                                            
 * ----------------------------------------------------------------------------            
 * Email            :   contacto@arkoquantum.com                                              
 * Website          :   arkoquantum.com                                                        
 *                                                                             
 * Copyright � ARKO Quantum S.R.L. 2025. All rights reserved.                            
 *****************************************************************************/

/******************************************************************************
 * Description:
 *  - Simple Time Based Opening Range Breakout System by ARKO Quantum S.R.L.
 *  
 *  - Supports:
 *  --> Auto break even function with fee coverage calculation
 *  --> Automatic output for monitoring and tracking
 *  --> Automatic account flattening at a specified time
 *  
 *  - Optimizable:
 *  --> OR length
 *  --> Breakeven value
 *
 * Strategy Version: 1.0                        
 *****************************************************************************/

#region ARKO Quantum TBORB System
namespace NinjaTrader.NinjaScript.Strategies.ARKOQuantum
{
    [Gui.CategoryOrder("General Settings", 1)]
    [Gui.CategoryOrder("Position Management", 1)]
    public class AQDFCSITBORB : Strategy
	{
        #region Variables
        // Strategy nickname.
        private string nickname;

        // Opening range values.
        private double openingRangeHigh;
        private double openingRangeLow;
        private double openingRangeValue;

        // Flag variables.
        private bool isOpeningRangeActive           = false;
        private bool hasTradedToday                 = true;
        private bool hasAutoBreakevenSet            = true;

        // Datetime variables.
        private DateTime currentTime;

        // Order variables.
        private Order longEntryOrder                = null;
        private Order shortEntryOrder               = null;
        private Order stopLossOrder                 = null;
        private Order autoBreakevenstopLossOrder    = null;
        private Order closeLongOrder                = null;
        private Order closeShortOrder               = null;

        // Trade variables.
        private Trade lastTrade                     = null;
        #endregion

        #region Settings
        protected override void OnStateChange()
		{
            if (State == State.SetDefaults)
			{
				Description									= @"Time based opening range breakout strategy";
				Name										= "AQDFCSITBORB";
                nickname                                    = "TBORB"; 
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 60;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
                IncludeCommission                           = true;
				Slippage									= 0;
				StartBehavior								= StartBehavior.AdoptAccountPosition;
                TimeInForce                                 = TimeInForce.Day;
				TraceOrders									= false;
                RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 5;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= false;
                // Enables usage of customized orders
                IsUnmanaged                                 = true;
				
                #region Parameters
                // Historical testing parameters
                HistoricalTests                             = false;

                // Time period parameters.
                startTime									= DateTime.Parse("08:31", System.Globalization.CultureInfo.InvariantCulture);
				endTime										= DateTime.Parse("08:35", System.Globalization.CultureInfo.InvariantCulture);

                // Trade management parameters.
                positionSize                                = 1;

                // ATM switch.
                advancedTradeManagementSwitch               = false;

                // Time-based flattening parameters.
                flattenBy                                   = false;
                flattenByTime                               = DateTime.Parse("15:00", System.Globalization.CultureInfo.InvariantCulture);

                // Auto break even parameters. 
                autoBreakeven                               = true;
                autoBreakevenValue                          = 1;
                feeCoverage                                 = 0;
                #endregion

            }
			else if (State == State.Configure)
			{
			}
            else if (State == State.DataLoaded)
            {
				ClearOutputWindow();
            }
            else if (State == State.Realtime)
            {
                Print($"{Time[0].TimeOfDay}, {Instrument.FullName}: ARKO Quantum {nickname} Active");             
            }

        }
        #endregion

        #region Logic
        protected override void OnBarUpdate()
		{
            #region Requirements
           	if (!HistoricalTests)
            { // Ensures historical data is not taken into account when unchecked

                if (State != State.Realtime) return;
            }
            if (CurrentBar < BarsRequiredToTrade) return;
            #endregion

            #region Entry Mechanism
            currentTime = Times[0][0];
            
            if (currentTime.TimeOfDay >= startTime.TimeOfDay && currentTime.TimeOfDay <= endTime.TimeOfDay)
            { // Initiate and capture opening range highest and lowest prices.

                if (!isOpeningRangeActive)
                { // First part initiates the count.

                    openingRangeHigh            = Highs[0][0];
                    openingRangeLow             = Lows[0][0];

                    Print("");
					Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> Opening Range Initiated: {Time[0].ToLongDateString()}");

                    isOpeningRangeActive        = true;
                    hasTradedToday              = false;
                    hasAutoBreakevenSet         = false;
                }
                else
                { // Second part completes the count by updating the values.

                    openingRangeHigh = Math.Max(openingRangeHigh, Highs[0][0]);
                    openingRangeLow = Math.Min(openingRangeLow, Lows[0][0]);
                }
            }
         
            if (currentTime.TimeOfDay > endTime.TimeOfDay && !hasTradedToday)
            { // Assign threshold prices and set pending orders.

                // Opening range completed for the day.
                isOpeningRangeActive = false; 

                // Ensures orders are not rejected due to invalid prices.
                openingRangeHigh    = Math.Max(openingRangeHigh, GetCurrentAsk() + (2 * TickSize));
                openingRangeLow     = Math.Min(openingRangeLow, GetCurrentBid() - (2 * TickSize));
                openingRangeValue   = openingRangeHigh - openingRangeLow;

                Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> Opening Range Completed: High = {openingRangeHigh}, Low = {openingRangeLow}, Range = {openingRangeValue}");

                // Set stop limit entry orders. Whenever one is executed, the other acts as a stop loss.
                longEntryOrder = SubmitOrderUnmanaged(0, OrderAction.Buy, OrderType.StopMarket, positionSize, 0, openingRangeHigh, "", "Long Stop Entry");
                shortEntryOrder = SubmitOrderUnmanaged(0, OrderAction.Sell, OrderType.StopMarket, positionSize, 0, openingRangeLow, "", "Short Stop Entry");

                hasTradedToday  = true;
            }
            #endregion

            #region Advanced Trade Management
            if (advancedTradeManagementSwitch && Position.MarketPosition != MarketPosition.Flat)
            { // Activates whenever account position is not flat and user aenabled advanced trade management.

                if (Position.MarketPosition == MarketPosition.Long)
                { // For long positions.

                    #region Long Auto Break Even Mechanism
                    if (autoBreakeven && !hasAutoBreakevenSet)
                    {
                        // Checks if unrealized gain is higher than specified minimum before implementing auto breakeven.
                        if (Position.GetUnrealizedProfitLoss(PerformanceUnit.Points, Close[0]) >= (autoBreakevenValue * openingRangeValue))
                        {
                            // Disables previous pending orders and submits updated stop loss order.
                            CancelOrder(longEntryOrder);
                            CancelOrder(shortEntryOrder);
                            CancelOrder(stopLossOrder);

                            autoBreakevenstopLossOrder = SubmitOrderUnmanaged(0, OrderAction.Sell, OrderType.StopMarket, positionSize, 0, Position.AveragePrice + (feeCoverage * TickSize), "", "Long autoBreakeven Order");
                            Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> ABE Order Set: {positionSize} @ {Position.AveragePrice + (feeCoverage * TickSize)}");

                            hasAutoBreakevenSet = true;
                        }
                    }
                    #endregion

                    #region  Flatten by mechanism
                    if (flattenBy && currentTime.TimeOfDay == flattenByTime.TimeOfDay)
                    {
                        closeLongOrder = SubmitOrderUnmanaged(0, OrderAction.Sell, OrderType.Market, positionSize, 0, 0, "", "Close Order"); ;

                        CancelOrder(longEntryOrder);
                        CancelOrder(shortEntryOrder);
                        CancelOrder(stopLossOrder);
                        CancelOrder(autoBreakevenstopLossOrder);
                    }
                    #endregion

                }
                else if (Position.MarketPosition == MarketPosition.Short)
                { // For short positions.

                    #region Short Auto Break Even Mechanism
                    if (autoBreakeven && !hasAutoBreakevenSet)
                    {
                        // Checks if unrealized gain is higher than specified minimum before implementing auto breakeven.
                        if (Position.GetUnrealizedProfitLoss(PerformanceUnit.Points, Close[0]) >= (autoBreakevenValue * openingRangeValue))
                        {
                            // Disables previous pending orders and submits updated stop loss order.
                            CancelOrder(longEntryOrder);
                            CancelOrder(shortEntryOrder);
                            CancelOrder(stopLossOrder);

                            autoBreakevenstopLossOrder = SubmitOrderUnmanaged(0, OrderAction.Buy, OrderType.StopMarket, positionSize, 0, Position.AveragePrice - (feeCoverage * TickSize), "", "Long autoBreakeven");
                            Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> ABE Order Set: {positionSize} @ {Position.AveragePrice - (feeCoverage - TickSize)}");

                            hasAutoBreakevenSet = true;
                        }
                    }
                    #endregion

                    #region  Flatten by mechanism
                    if (flattenBy && currentTime.TimeOfDay == flattenByTime.TimeOfDay)
                    {
                        closeShortOrder = SubmitOrderUnmanaged(0, OrderAction.Buy, OrderType.Market, positionSize, 0, 0, "", "Close Order"); ;

                        CancelOrder(longEntryOrder);
                        CancelOrder(shortEntryOrder);
                        CancelOrder(stopLossOrder);
                        CancelOrder(autoBreakevenstopLossOrder);
                    }
                    #endregion
                }
            }
            #endregion

            #region Stop Loss Display
            if (Position.MarketPosition != MarketPosition.Flat && hasAutoBreakevenSet)
                Draw.Line(this, CurrentBar + "Stop", true, 1, autoBreakevenstopLossOrder.StopPrice, 0, autoBreakevenstopLossOrder.StopPrice, Brushes.Red, DashStyleHelper.Solid, 2);
            #endregion
                
        }
        #endregion

        #region Monitor
        protected override void OnPositionUpdate(Position position, double averagePrice, int quantity, MarketPosition marketPosition)
        { // Prints live information about executions to monitor systems.
            if (Position.MarketPosition == MarketPosition.Long)
                Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> Long entry: {quantity} @ {averagePrice}");

            if (Position.MarketPosition == MarketPosition.Short)
                Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> Short entry: {quantity} @ {averagePrice}");

            if (Position.MarketPosition == MarketPosition.Flat)
            {
                lastTrade = SystemPerformance.AllTrades[SystemPerformance.AllTrades.Count - 1];

                Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> Account Flattened @ {lastTrade.Exit.Price}");
                Print($"{Time[0].TimeOfDay}: {nickname}, {Instrument.FullName} -> Realized: {lastTrade.ProfitPoints} points");
            }
        }
        #endregion

        #region Properties
        [NinjaScriptProperty]
        [Display(Name = "Historical Testing", Description = "Allows historical testing. Disable before running live.", Order = 1, GroupName = "General Settings")]
        public bool HistoricalTests
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Position Size", Description = "Amount of contracts to be traded.", Order = 2, GroupName = "General Settings")]
        public int positionSize
        { get; set; }

        [NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="OR Start Time", Description="Opening range start time.", Order = 3, GroupName = "General Settings")]
		public DateTime startTime
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="OR End Time", Description="Opening range end time.", Order = 4, GroupName = "General Settings")]
		public DateTime endTime
		{ get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ATM Switch", Description = "Advanced trade management system. If unchecked, flatten by and abe functions will be disabled.",Order = 1, GroupName = "Position Management")]
        public bool advancedTradeManagementSwitch
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Flatten By", Description = "Clear all open positions and pending orders at a specified time.", Order = 2, GroupName = "Position Management")]
        public bool flattenBy
        { get; set; }

        [NinjaScriptProperty]
        [PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
        [Display(Name = "Flatten Time", Description = "Time to flatten account.", Order = 3, GroupName = "Position Management")]
        public DateTime flattenByTime
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ABE", Description = "Auto break even functionality", Order = 4, GroupName = "Position Management")]
        public bool autoBreakeven
        { get; set; }

        [NinjaScriptProperty]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "ABE Value", Description = "Auto break even value as % of OR. Calculated on unrealized gain. Will be ignored if 'ABE'  is unchecked.", Order = 5, GroupName = "Position Management")]
        public double autoBreakevenValue
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, double.MaxValue)]
        [Display(Name = "Fee Coverage", Description = "Specify the amount of ticks to cover fees with auto break even.", Order = 6, GroupName = "Position Management")]
        public double feeCoverage
        { get; set; }
        #endregion
    }
}
#endregion
