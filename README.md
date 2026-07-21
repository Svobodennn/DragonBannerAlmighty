# Dragon Banner Almighty

A small Mount & Blade II: Bannerlord mod that makes the legendary **Dragon Banner**
(`dragon_banner`) grant **every vanilla banner effect at once**, with the strength of each
effect adjustable in-game via MCM.

## What it does

In vanilla a banner item can only carry a *single* banner effect, and the quest Dragon Banner
carries none. This mod gives the Dragon Banner **all 13** banner effects simultaneously and lets
you tune each one independently:

Increased melee damage · Increased melee damage vs mounted · Increased ranged damage ·
Increased charge damage · Decreased incoming charge damage · Decreased ranged accuracy penalty ·
Decreased morale shock · Decreased incoming melee damage · Decreased incoming ranged damage ·
Decreased shield damage · Increased troop movement speed · Increased mount movement speed ·
Increased morale shock dealt by melee troops.

Hovering the Dragon Banner in your inventory lists every active effect with its current value.

## How detection works

Effects apply to **your troops** whenever **your party holds the assembled `dragon_banner`**
(in the party inventory or the main hero's banner slot) — you do **not** need to assign a
banner-bearer. As a bonus it also honours the normal mechanic: if a formation actually carries the
Dragon Banner as its active battle banner, that formation gets the effects too.

Under the hood it Postfixes the concrete Sandbox combat models (damage, agent stats, morale) rather
than the vanilla banner helper (which is inlined and cannot be patched directly). Every effect is a
percentage (AddFactor) bonus, applied as `value *= (1 + tierValue * multiplier)`.

## Configuration (MCM)

Options → Mod Options → **Dragon Banner Almighty**:

- **Enable Mod (Master Switch)** — master on/off for all effects (default on).
- One group **per effect**, each with:
  - **Enabled** — turn that single effect on/off.
  - **Tier (1–3)** — which vanilla banner tier's value the effect uses (default **3**, strongest).
  - **Multiplier (0.1–10)** — scales that effect on top of the tier (default **1.0**).

## Requirements / load order

`Bannerlord.Harmony` → `Bannerlord.ButterLib` → `Bannerlord.MCM` (MBOptionScreen) →
Native / SandBoxCore / Sandbox / StoryMode → **DragonBannerAlmighty**.
(BLSE "Auto Sort" produces this order automatically.)

Built against game **v1.4.5** using the stable Sandbox model API, so it should also run on newer
1.4.x. This is a **net472 / Win64_Shipping_Client** build (Steam / GOG / Epic).

## Install

1. Copy the `DragonBannerAlmighty` folder into your game's `Modules/` directory.
2. In the launcher, enable **DragonBannerAlmighty** (after Harmony, ButterLib and MCM).
3. Launch the game through **BLSE**.

## Build from source

```bash
dotnet build -c Release -p:Platform=x64
# -> assembles dist/Modules/DragonBannerAlmighty/ (SubModule.xml + bin/Win64_Shipping_Client/DragonBannerAlmighty.dll)
```

## Clean-room

This is a clean-room implementation. It does **not** use, contain, or redistribute any files from
any other mod.
