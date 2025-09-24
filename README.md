# AQTBORB - Time Based Opening Range Breakout System

A Custom **NinjaTrader 8 automated strategy** developed by **ARKO Quantum S.R.L.**  

This system is designed to capture intraday breakouts based on a user-defined, time-based, opening range window. It automatically places stop market orders above and below the range and includes advanced position management tools such as **auto breakeven stops** and **scheduled account flattening**.  

The system is suitable for both **live execution** and **backtesting**, with optimizable parameters to help refine performance through diverse optimization processes such as standard and walk forward optimizations, Monte Carlo simulations, and Genetic Analysis.

---

## Features
- Opening Range Breakout Logic  
  Actively captures the high and low extremes of price within a defined time window and submits stop orders immediatelly after the window comes to an end. This approach ensures accurate tracking of the price block and guarantees coverage regardless of the window lentgth  

- Configurable Parameters  
  - Opening range start & end times  
  - Position sizing  
  - Auto break-even value (% of OR)  
  - Fee coverage in ticks  
  - Flatten-by time (optional)  

- Trade Management Options  
  - Auto break even with fee coverage  
  - Time-based account flattening  
  - Advanced Trade Management (ATM) switch  

- Optimizable for Strategy Analyzer  
  - Opening range length  
  - Auto breakeven thresholds  

- Safety Controls  
  - Unmanaged order handling for precise execution  
  - Historical test mode toggle  
  - Real-time execution output for monitoring  

---

## Installation
1. Download the compiled **ZIP file** from this repository.  
2. Open **NinjaTrader 8**.  
3. Go to: `Tools → Import → NinjaScript Add-On`.  
4. Select the downloaded **ZIP file** (`AQDFCSITBORB.zip`).  
5. Once imported, compile the script (`F5`).  
6. You will now see **AQDFCSITBORB** available under strategies in the platform.

Note: If you do not wish to import the compiled .zip, you must download the source code under /src, place it into the ninjatrader data folder within your mahchine, and proceed to compile it manually, after which, it will be accesible in the platform.

---

## Usage
1. Open a **Chart** or the **Strategy Analyzer**.  
2. Apply the strategy:  
   - Right-click → **Strategies** → Select `AQDFCSITBORB`.
3. Configure parameters:  
   - Set **Opening Range Start** & **End Times**  
   - Choose **Position Size**  
   - Enable/disable **Auto Break Even (ABE)**  
   - Enable **Flatten By Time** if desired  
   - Adjust **Fee Coverage** (ticks to cover commissions)  
4. Enable the strategy.  
5. Monitor the **Output Window** for real-time logs and order execution messages.  

---

## Examples

...

---

## License
This project is licensed under the **GNU General Public License v3.0 (GPL-3.0)**.  

You are free to use, modify, and distribute this code under the terms of the license.  
