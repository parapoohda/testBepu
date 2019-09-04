using DemoRenderer.UI;
using DemoUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace bepuphysics2_for_nelalen.UI
{
    struct DemoSwapper
    {
        public int TargetDemoIndex;
        bool TrackingInput;

        /*public void CheckForDemoSwap(DemoHarness harness)
        {
            if (harness.input.WasPushed(harness.controls.ChangeDemo.Key))
            {
                TrackingInput = !TrackingInput;
                TargetDemoIndex = -1;
            }

            if (TrackingInput)
            {
                for (int i = 0; i < harness.input.TypedCharacters.Count; ++i)
                {
                    var character = harness.input.TypedCharacters[i];
                    if (character == '\b')
                    {
                        //Backspace!
                        if (TargetDemoIndex >= 10)
                            TargetDemoIndex /= 10;
                        else
                            TargetDemoIndex = -1;
                    }
                    else
                    {
                        if (TargetDemoIndex < harness.demoSet.Count)
                        {
                            var digit = character - '0';
                            if (digit >= 0 && digit <= 9)
                            {
                                TargetDemoIndex = Math.Max(0, TargetDemoIndex) * 10 + digit;
                            }
                        }
                    }
                }

                if (harness.input.WasPushed(OpenTK.Input.Key.Enter))
                {
                    //Done entering the index. Swap the demo if needed.
                    TrackingInput = false;
                    harness.TryChangeToDemo(TargetDemoIndex);
                }
            }

        }*/

        /*public void Draw(TextBuilder text, TextBatcher textBatcher, String StringOfMap, Vector2 position, float textHeight, Vector3 textColor, Font font)
        {
            if (TrackingInput)
            {
                text.Clear().Append("Swap demo to: ");
                if (TargetDemoIndex >= 0)
                    text.Append(TargetDemoIndex);
                else
                    text.Append("_");
                textBatcher.Write(text, position, textHeight, textColor, font);

                var lineSpacing = textHeight * 1.1f;
                position.Y += textHeight * 0.5f;
                textHeight *= 0.8f;
                position.Y += lineSpacing;
                text.Clear().Append(StringOfMap);
                textBatcher.Write(text.Clear().Append(i).Append(": ").Append(StringOfMap), position, textHeight, textColor, font);
                
            }

        }*/

    }
}
