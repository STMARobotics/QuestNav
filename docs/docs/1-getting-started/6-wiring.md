---
title: Wiring 
---
# Wiring

Proper wiring is essential for reliable communication between your Quest headset and robot. This guide covers Ethernet connections, power options, and best practices for secure wiring.

## Ethernet Connection

QuestNav requires a direct Ethernet connection between your Quest headset and the robot's network.

:::info
Per FRC rules, wireless communication is not allowed within the robot. The Ethernet connection ensures compliance with competition rules. See the [current game manual](https://www.firstinspires.org/resource-library/frc/competition-manual-qa-system) for details.
:::

### Basic Setup

1. Connect your USB-C to Ethernet adapter to the Quest headset
2. Connect an Ethernet cable between the adapter and your robot's network switch
3. Use the shortest cable possible to minimize signal loss and physical interference

### Cable Selection

- **Cable Type**: CAT5e or CAT6 Ethernet cable
- **Length**: Ideally under 3 feet (1 meter)
- **Shielded**: Recommended in high-EMI environments
- **Strain Relief**: Consider right-angle connectors if space is limited

:::tip
Shielded Ethernet cables (STP) provide better resistance to electromagnetic interference from motors and other robot components compared to unshielded (UTP) cables.
:::

## Power Options

The Quest headset should be powered using a **USB battery bank** mounted on the robot. This provides clean, stable 5V power independent of the robot's electrical system.

### Recommended Setup

1. Mount a 5V USB battery bank securely on the robot
2. Connect a USB-A to USB-C cable from the battery bank to your adapter's power input
3. The battery bank should supply enough power to sustain the Quest headset indefinitely
4. Charge state can be monitored externally using the power meter on the battery bank

:::danger
Only use 5V output from power banks. Avoid banks that support USB-C Power Delivery (PD) or use a USB-A to USB-C cable to force 5V delivery. Some adapters will boot loop when voltage greater than 5V is applied.
:::

### Power Requirements

- **Voltage**: 5V DC ±5%
- **Current**: 2-3A recommended
- **Connector**: USB-C

### Without Power Passthrough

If your adapter does not support power passthrough:

1. Fully charge the Quest headset before each match
2. The Quest battery typically lasts 2-3 hours in QuestNav mode

:::warning
Running on the Quest's internal battery alone is not recommended for competition use due to the risk of battery depletion during long events.
:::

## Wiring Best Practices

Follow these guidelines to ensure reliable operation:

### Secure Connections

- Use zip ties or cable clips to secure cables to the robot frame
- Leave small service loops at connection points to prevent tension
- Avoid tight bends in cables (maintain at least 1" bend radius)
- Label cables for easy identification during maintenance

:::info
Service loops are small, intentional slack sections in the cable that prevent tension from being applied directly to connectors. They're essential for preventing connection failures during robot movement.
:::

### Redundancy

- Consider having a backup Ethernet cable and adapter ready
- Create quick-disconnect points for faster field repairs
- Test connections before each match

:::tip
Create a simple checklist to verify all connections before matches. A quick visual inspection can catch loose cables before they cause problems.
:::

## Troubleshooting Common Issues

### No network connection
- Check adapter LED indicators
- Try a different Ethernet cable
- Verify the adapter is properly seated in the Quest's USB-C port

:::note
Most Ethernet adapters have status LEDs that indicate link and activity. No lights usually indicates a power or connection issue.
:::

### Intermittent connection
- Look for loose connectors or cable damage
- Ensure cables are properly secured to prevent movement

### Headset not charging
- Verify power supply output and connections
- Check for bent pins in the USB-C connector
- Test with a known working power source

:::danger
If your Quest is losing charge during operation despite being connected to power, check your power source immediately. Running out of battery during a match is very bad!
:::

## Video Guide
:::tip Video Guide
A video walkthrough for wiring is coming soon.
:::

## Next Steps
With your Quest properly wired, proceed to the [Robot Code Setup](./robot-code) section to configure your robot's software for QuestNav integration.
