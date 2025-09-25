# ARKO Quantum Time Based Opening Range Breakout

A Custom **NinjaTrader 8** automated strategy developed by **ARKO Quantum S.R.L.** for the American futures market.

This system is designed to capture intraday breakouts within a user-defined, time-based opening range. It automatically places stop market orders above and below the range and incorporates advanced position management features, including auto breakeven stops and scheduled account flattening.

TBORB places strong emphasis on the timeliness of the selected window or time period. Unlike other systems with a similar methodology, this algorithm does not incorporate a volatility factor when calculating stop prices. Instead, it uses the high and low of the opening range as thresholds, which function in practice as market exposure triggers.

Observers will realize that the system’s structure resembles a long straddle, where the focus is on capturing movements rather than predicting direction. In TBORB, once a threshold is crossed, the subsequent move is largely driven by market microstructure dynamics. Specifically, the depth of the order book often becomes unbalanced or asymmetrical, with limited opposing liquidity, allowing the price to continue moving in the breakout direction with minimal resistance. This mechanism operates independently of the move’s direction, making the approach effectively market-neutral, as it is designed to capture breakouts regardless of whether the market moves up or down.

The system is suitable for both live execution and backtesting, with fully optimizable parameters. Performance can be refined through a range of processes, including standard and walk-forward optimizations, Monte Carlo simulations, and genetic analysis.

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

![Alt text](/assets/screenshot_1.jpg?raw=true "screenshot 1")

![Alt text](/assets/screenshot_2.jpg?raw=true "screenshot 2")

![Alt text](/assets/screenshot_3.jpg?raw=true "screenshot 3")

![Alt text](/assets/screenshot_4.jpg?raw=true "screenshot 4")

![Alt text](/assets/screenshot_5.jpg?raw=true "screenshot 5")

---

## References

The development of AQTBORB was inspired by the following research:

1. Holmberg, Ulf, et al. *Assessing the Profitability of Intraday Opening Range Breakout Strategies*. Umeå University, 2012. [PDF](https://umu.diva-portal.org/smash/record.jsf?pid=diva2%3A553015)

2. Lundström, Christian. *Day Trading Returns Across Volatility States*. Umeå University, 2017. [PDF](https://www.diva-portal.org/smash/get/diva2%3A732318/FULLTEXT02.pdf)

3. Wu, Mu-En, et al. *Evolutionary ORB-Based Model with Protective Closing Strategies*. Elsevier, 2021. [Link](https://www.sciencedirect.com/science/article/pii/S0950705121000320)

4. Wang, Chia-Jung, et al. *Assessing the Profitability of Timely Opening Range Breakout on Index Futures Markets*. IEEE Access, vol. 7, 2019, pp. 32061–32071. [Link](https://ntut.elsevierpure.com/en/publications/assessing-the-profitability-of-timely-opening-range-breakout-on-i)

5. Syu, Jui-Hsiang, et al. *Modified ORB Strategies with Threshold Adjusting on Taiwan Futures Market*. 2019. [Link](https://www.researchgate.net/publication/334427771_Modified_ORB_Strategies_with_Threshold_Adjusting_on_Taiwan_Futures_Market)

6. Wu, Mu-En, et al. *A Profitable Day Trading Strategy*. 2024. [PDF](https://papers.ssrn.com/sol3/Delivery.cfm/4729284.pdf?abstractid=4729284&mirid=1)

7. Wu, Mu-En, et al. *Can Day Trading Really Be Profitable?*. 2024. [PDF](https://papers.ssrn.com/sol3/Delivery.cfm/SSRN_ID2488539_code1009018.pdf?abstractid=2488539&mirid=1)

> **Note:** While none of these papers was used individually to develop AQTBORB, the system was inspired by a comprehensive review of the research landscape. We synthesized insights from various studies to create our proprietary tool, integrating elements from multiple strategies and methodologies.

---

## License
This project is licensed under the **GNU General Public License v3.0 (GPL-3.0)**.  

You are free to use, modify, and distribute this code under the terms of the license.  
