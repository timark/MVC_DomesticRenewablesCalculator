# Domestic Renewables ROI Calculator

This ASP.NET Core MVC application helps households evaluate the return on investment (ROI) of solar PV, battery storage, and hot water load shifting. Unlike simple calculators, it considers how energy can be shifted into cheaper tariff periods and how occupant usage patterns influence self-consumption.

## Features

- **Flexible tariff modelling** – capture peak, shoulder, off-peak, and feed-in rates plus daily supply charges.
- **Occupant usage profiles** – describe average consumption and how it is distributed across tariff periods and daylight hours.
- **Solar, battery, and hot water inputs** – toggle technologies on/off and provide cost, performance, and lifetime assumptions for each asset.
- **Advanced ROI metrics** – compute annual savings, payback period, net present value (NPV), and long-term ROI over a configurable analysis horizon.
- **Energy flow breakdown** – visualise how much load is met by solar, batteries, and thermal storage while accounting for off-peak charging and storage losses.

## Modelling approach

1. **Baseline energy cost** – annual consumption is split across peak, shoulder, and off-peak periods based on the occupant profile, then multiplied by tariff rates.
2. **Solar contribution** – solar generation is estimated from system size and average daily production. Daytime usage defines how much solar is consumed on-site, with any surplus exported.
3. **Battery behaviour** – the battery first stores excess solar (reducing exports) and, if capacity remains, can charge from off-peak tariffs to offset peak and shoulder usage. Round-trip efficiency and depth of discharge are applied to keep results realistic.
4. **Hot water shifting** – thermal storage moves water-heating loads from peak/shoulder to off-peak periods while accounting for storage efficiency losses.
5. **Financial analysis** – annual operating cost (including maintenance and feed-in revenue) is compared to the baseline. Upfront costs, savings, and the selected discount rate yield NPV, simple payback, and ROI over the analysis period.

## Getting started

1. Install the [.NET 7 SDK](https://dotnet.microsoft.com/download).
2. Navigate to the project folder and restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the development server:
   ```bash
   dotnet run --project DomesticRenewablesCalculator
   ```
4. Open `https://localhost:7123` (or the URL shown in the console) to use the calculator.

## Next steps

- Replace the simple average-day modelling with detailed interval load and generation data to further personalise ROI estimates.
- Introduce scenario comparison so that users can quickly evaluate multiple system configurations side-by-side.
- Persist inputs with a lightweight database or browser storage to make iterative analysis easier.

