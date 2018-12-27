//自动生成代码，不要修改
using System;
using System.Collections.Generic;
using FlatNode.Runtime.Flat;

namespace FlatNode.Runtime
{
    public static partial class FlatNodeUtility
    {
        private static Dictionary<string, Func<NodeBase>> nodeFactoryDictionary;
        
        private static void InitNodeFactoryDictionary()
        {
            if (nodeFactoryDictionary != null){return;}
            
            nodeFactoryDictionary = new Dictionary<string, Func<NodeBase>>
            {
                {"FlatNode.Runtime.CreateSequenceNode",()=> new CreateSequenceNode()},
                {"FlatNode.Runtime.LogNode",()=> new LogNode()},
                {"FlatNode.Runtime.OnClickNode",()=> new OnClickNode()},
                {"FlatNode.Runtime.OnJoyStickDownNode",()=> new OnJoyStickDownNode()},
                {"FlatNode.Runtime.OnJoyStickDragNode",()=> new OnJoyStickDragNode()},
                {"FlatNode.Runtime.OnJoyStickUpNode",()=> new OnJoyStickUpNode()},
                {"FlatNode.Runtime.IfNode",()=> new IfNode()},
                {"FlatNode.Runtime.RepeatNode",()=> new RepeatNode()},
                {"FlatNode.Runtime.StopRunningNode",()=> new StopRunningNode()},
                {"FlatNode.Runtime.UpdateNode",()=> new UpdateNode()},
                {"FlatNode.Runtime.WaitNode",()=> new WaitNode()},
                {"FlatNode.Runtime.IntComparerNode",()=> new IntComparerNode()},
                {"FlatNode.Runtime.InverseDirectionNode",()=> new InverseDirectionNode()},
                {"FlatNode.Runtime.JoyStickInputConvertNode",()=> new JoyStickInputConvertNode()},
                {"FlatNode.Runtime.NormalizeVectorNode",()=> new NormalizeVectorNode()},
                {"FlatNode.Runtime.QuadraticBezierPathNode",()=> new QuadraticBezierPathNode()},
                {"FlatNode.Runtime.RandomFloatNode",()=> new RandomFloatNode()},
                {"FlatNode.Runtime.VectorAddNode",()=> new VectorAddNode()},
                {"FlatNode.Runtime.VectorScalarNode",()=> new VectorScalarNode()},
                {"FlatNode.Runtime.SetVector3ComponentNode",()=> new SetVector3ComponentNode()},
                {"FlatNode.Runtime.TestNode",()=> new TestNode()},
                {"FlatNode.Runtime.GetVariableNode",()=> new GetVariableNode()},
                {"FlatNode.Runtime.SetVariableNode",()=> new SetVariableNode()},
            };
        }
        
        private static Dictionary<Type, Action<NodeBase, NodeInputFieldInfo>> inputPortParseFuncDictionary;
        
        private static void InitInputPortParseFuncsDictionary()
        {
            if (inputPortParseFuncDictionary != null){return;}
            
            inputPortParseFuncDictionary = new Dictionary<Type, Action<NodeBase, NodeInputFieldInfo>>
            {
                {
                    typeof(CreateSequenceNode),(nodeInstance, inputFieldInfo) =>
                    {
                        CreateSequenceNode createSequenceNode = nodeInstance as CreateSequenceNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "initCountInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                createSequenceNode.initCountInputVariable = new NodeInputVariable<int>();
                                createSequenceNode.initCountInputVariable.targetNodeId = targetNodeId;
                                createSequenceNode.initCountInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                createSequenceNode.initCountInputVariable = new NodeInputVariable<int>();
                                createSequenceNode.initCountInputVariable.value = int.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(LogNode),(nodeInstance, inputFieldInfo) =>
                    {
                        LogNode logNode = nodeInstance as LogNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "logTypeInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                logNode.logTypeInputVariable = new NodeInputVariable<FlatNode.Runtime.LogNode.LogType>();
                                logNode.logTypeInputVariable.targetNodeId = targetNodeId;
                                logNode.logTypeInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                logNode.logTypeInputVariable = new NodeInputVariable<FlatNode.Runtime.LogNode.LogType>();
                                logNode.logTypeInputVariable.value = (FlatNode.Runtime.LogNode.LogType)Enum.Parse(typeof(FlatNode.Runtime.LogNode.LogType), valueString);
                            }
                        }
                        else if (inputPortFieldName == "logStringInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                logNode.logStringInputVariable = new NodeInputVariable<string>();
                                logNode.logStringInputVariable.targetNodeId = targetNodeId;
                                logNode.logStringInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                logNode.logStringInputVariable = new NodeInputVariable<string>();
                                logNode.logStringInputVariable.value = valueString;
                            }
                        }
                    }
                },
                {
                    typeof(IfNode),(nodeInstance, inputFieldInfo) =>
                    {
                        IfNode ifNode = nodeInstance as IfNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "boolInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                ifNode.boolInput = new NodeInputVariable<bool>();
                                ifNode.boolInput.targetNodeId = targetNodeId;
                                ifNode.boolInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                ifNode.boolInput = new NodeInputVariable<bool>();
                                ifNode.boolInput.value = Boolean.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(RepeatNode),(nodeInstance, inputFieldInfo) =>
                    {
                        RepeatNode repeatNode = nodeInstance as RepeatNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "repeatCountInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                repeatNode.repeatCountInputVariable = new NodeInputVariable<int>();
                                repeatNode.repeatCountInputVariable.targetNodeId = targetNodeId;
                                repeatNode.repeatCountInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                repeatNode.repeatCountInputVariable = new NodeInputVariable<int>();
                                repeatNode.repeatCountInputVariable.value = int.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "repeatIntervalSecondsInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                repeatNode.repeatIntervalSecondsInputVariable = new NodeInputVariable<float>();
                                repeatNode.repeatIntervalSecondsInputVariable.targetNodeId = targetNodeId;
                                repeatNode.repeatIntervalSecondsInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                repeatNode.repeatIntervalSecondsInputVariable = new NodeInputVariable<float>();
                                repeatNode.repeatIntervalSecondsInputVariable.value = float.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "executeImmediatelyInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                repeatNode.executeImmediatelyInputVariable = new NodeInputVariable<bool>();
                                repeatNode.executeImmediatelyInputVariable.targetNodeId = targetNodeId;
                                repeatNode.executeImmediatelyInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                repeatNode.executeImmediatelyInputVariable = new NodeInputVariable<bool>();
                                repeatNode.executeImmediatelyInputVariable.value = Boolean.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(StopRunningNode),(nodeInstance, inputFieldInfo) =>
                    {
                        StopRunningNode stopRunningNode = nodeInstance as StopRunningNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "stopNodeIdsInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                stopRunningNode.stopNodeIdsInput = new NodeInputVariable<List<int>>();
                                stopRunningNode.stopNodeIdsInput.targetNodeId = targetNodeId;
                                stopRunningNode.stopNodeIdsInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                stopRunningNode.stopNodeIdsInput = new NodeInputVariable<List<int>>();
                                stopRunningNode.stopNodeIdsInput.value = ParseIntList(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(UpdateNode),(nodeInstance, inputFieldInfo) =>
                    {
                        UpdateNode updateNode = nodeInstance as UpdateNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "intervalInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                updateNode.intervalInput = new NodeInputVariable<float>();
                                updateNode.intervalInput.targetNodeId = targetNodeId;
                                updateNode.intervalInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                updateNode.intervalInput = new NodeInputVariable<float>();
                                updateNode.intervalInput.value = float.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(WaitNode),(nodeInstance, inputFieldInfo) =>
                    {
                        WaitNode waitNode = nodeInstance as WaitNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "waitTimeInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                waitNode.waitTimeInputVariable = new NodeInputVariable<float>();
                                waitNode.waitTimeInputVariable.targetNodeId = targetNodeId;
                                waitNode.waitTimeInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                waitNode.waitTimeInputVariable = new NodeInputVariable<float>();
                                waitNode.waitTimeInputVariable.value = float.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(IntComparerNode),(nodeInstance, inputFieldInfo) =>
                    {
                        IntComparerNode intComparerNode = nodeInstance as IntComparerNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "aInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                intComparerNode.aInput = new NodeInputVariable<int>();
                                intComparerNode.aInput.targetNodeId = targetNodeId;
                                intComparerNode.aInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                intComparerNode.aInput = new NodeInputVariable<int>();
                                intComparerNode.aInput.value = int.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "bInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                intComparerNode.bInput = new NodeInputVariable<int>();
                                intComparerNode.bInput.targetNodeId = targetNodeId;
                                intComparerNode.bInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                intComparerNode.bInput = new NodeInputVariable<int>();
                                intComparerNode.bInput.value = int.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "compareTypeInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                intComparerNode.compareTypeInput = new NodeInputVariable<FlatNode.Runtime.CompareType>();
                                intComparerNode.compareTypeInput.targetNodeId = targetNodeId;
                                intComparerNode.compareTypeInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                intComparerNode.compareTypeInput = new NodeInputVariable<FlatNode.Runtime.CompareType>();
                                intComparerNode.compareTypeInput.value = (FlatNode.Runtime.CompareType)Enum.Parse(typeof(FlatNode.Runtime.CompareType), valueString);
                            }
                        }
                    }
                },
                {
                    typeof(InverseDirectionNode),(nodeInstance, inputFieldInfo) =>
                    {
                        InverseDirectionNode inverseDirectionNode = nodeInstance as InverseDirectionNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "directionInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                inverseDirectionNode.directionInput = new NodeInputVariable<UnityEngine.Vector3>();
                                inverseDirectionNode.directionInput.targetNodeId = targetNodeId;
                                inverseDirectionNode.directionInput.targetPortId = targetPortId;
                            }
                        }
                    }
                },
                {
                    typeof(JoyStickInputConvertNode),(nodeInstance, inputFieldInfo) =>
                    {
                        JoyStickInputConvertNode joyStickInputConvertNode = nodeInstance as JoyStickInputConvertNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "vector2Input")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                joyStickInputConvertNode.vector2Input = new NodeInputVariable<UnityEngine.Vector2>();
                                joyStickInputConvertNode.vector2Input.targetNodeId = targetNodeId;
                                joyStickInputConvertNode.vector2Input.targetPortId = targetPortId;
                            }
                        }
                    }
                },
                {
                    typeof(NormalizeVectorNode),(nodeInstance, inputFieldInfo) =>
                    {
                        NormalizeVectorNode normalizeVectorNode = nodeInstance as NormalizeVectorNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "vector2Input")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                normalizeVectorNode.vector2Input = new NodeInputVariable<UnityEngine.Vector2>();
                                normalizeVectorNode.vector2Input.targetNodeId = targetNodeId;
                                normalizeVectorNode.vector2Input.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "vector3Input")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                normalizeVectorNode.vector3Input = new NodeInputVariable<UnityEngine.Vector3>();
                                normalizeVectorNode.vector3Input.targetNodeId = targetNodeId;
                                normalizeVectorNode.vector3Input.targetPortId = targetPortId;
                            }
                        }
                    }
                },
                {
                    typeof(QuadraticBezierPathNode),(nodeInstance, inputFieldInfo) =>
                    {
                        QuadraticBezierPathNode quadraticBezierPathNode = nodeInstance as QuadraticBezierPathNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "startPosInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                quadraticBezierPathNode.startPosInput = new NodeInputVariable<UnityEngine.Vector3>();
                                quadraticBezierPathNode.startPosInput.targetNodeId = targetNodeId;
                                quadraticBezierPathNode.startPosInput.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "endPosInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                quadraticBezierPathNode.endPosInput = new NodeInputVariable<UnityEngine.Vector3>();
                                quadraticBezierPathNode.endPosInput.targetNodeId = targetNodeId;
                                quadraticBezierPathNode.endPosInput.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "centerHeightOffsetInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                quadraticBezierPathNode.centerHeightOffsetInput = new NodeInputVariable<float>();
                                quadraticBezierPathNode.centerHeightOffsetInput.targetNodeId = targetNodeId;
                                quadraticBezierPathNode.centerHeightOffsetInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                quadraticBezierPathNode.centerHeightOffsetInput = new NodeInputVariable<float>();
                                quadraticBezierPathNode.centerHeightOffsetInput.value = float.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "spanLengthInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                quadraticBezierPathNode.spanLengthInput = new NodeInputVariable<float>();
                                quadraticBezierPathNode.spanLengthInput.targetNodeId = targetNodeId;
                                quadraticBezierPathNode.spanLengthInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                quadraticBezierPathNode.spanLengthInput = new NodeInputVariable<float>();
                                quadraticBezierPathNode.spanLengthInput.value = float.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(RandomFloatNode),(nodeInstance, inputFieldInfo) =>
                    {
                        RandomFloatNode randomFloatNode = nodeInstance as RandomFloatNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "minInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                randomFloatNode.minInputVariable = new NodeInputVariable<float>();
                                randomFloatNode.minInputVariable.targetNodeId = targetNodeId;
                                randomFloatNode.minInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                randomFloatNode.minInputVariable = new NodeInputVariable<float>();
                                randomFloatNode.minInputVariable.value = float.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "maxInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                randomFloatNode.maxInputVariable = new NodeInputVariable<float>();
                                randomFloatNode.maxInputVariable.targetNodeId = targetNodeId;
                                randomFloatNode.maxInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                randomFloatNode.maxInputVariable = new NodeInputVariable<float>();
                                randomFloatNode.maxInputVariable.value = float.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(VectorAddNode),(nodeInstance, inputFieldInfo) =>
                    {
                        VectorAddNode vectorAddNode = nodeInstance as VectorAddNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "aVectorInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                vectorAddNode.aVectorInput = new NodeInputVariable<UnityEngine.Vector3>();
                                vectorAddNode.aVectorInput.targetNodeId = targetNodeId;
                                vectorAddNode.aVectorInput.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "bVectorInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                vectorAddNode.bVectorInput = new NodeInputVariable<UnityEngine.Vector3>();
                                vectorAddNode.bVectorInput.targetNodeId = targetNodeId;
                                vectorAddNode.bVectorInput.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "typeInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                vectorAddNode.typeInput = new NodeInputVariable<FlatNode.Runtime.VectorAddNode.CalculateType>();
                                vectorAddNode.typeInput.targetNodeId = targetNodeId;
                                vectorAddNode.typeInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                vectorAddNode.typeInput = new NodeInputVariable<FlatNode.Runtime.VectorAddNode.CalculateType>();
                                vectorAddNode.typeInput.value = (FlatNode.Runtime.VectorAddNode.CalculateType)Enum.Parse(typeof(FlatNode.Runtime.VectorAddNode.CalculateType), valueString);
                            }
                        }
                    }
                },
                {
                    typeof(VectorScalarNode),(nodeInstance, inputFieldInfo) =>
                    {
                        VectorScalarNode vectorScalarNode = nodeInstance as VectorScalarNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "v2Input")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                vectorScalarNode.v2Input = new NodeInputVariable<UnityEngine.Vector2>();
                                vectorScalarNode.v2Input.targetNodeId = targetNodeId;
                                vectorScalarNode.v2Input.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "v3Input")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                vectorScalarNode.v3Input = new NodeInputVariable<UnityEngine.Vector3>();
                                vectorScalarNode.v3Input.targetNodeId = targetNodeId;
                                vectorScalarNode.v3Input.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "scalarInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                vectorScalarNode.scalarInput = new NodeInputVariable<float>();
                                vectorScalarNode.scalarInput.targetNodeId = targetNodeId;
                                vectorScalarNode.scalarInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                vectorScalarNode.scalarInput = new NodeInputVariable<float>();
                                vectorScalarNode.scalarInput.value = float.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(SetVector3ComponentNode),(nodeInstance, inputFieldInfo) =>
                    {
                        SetVector3ComponentNode setVector3ComponentNode = nodeInstance as SetVector3ComponentNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "originV3Input")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                setVector3ComponentNode.originV3Input = new NodeInputVariable<UnityEngine.Vector3>();
                                setVector3ComponentNode.originV3Input.targetNodeId = targetNodeId;
                                setVector3ComponentNode.originV3Input.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "targetComponentInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                setVector3ComponentNode.targetComponentInput = new NodeInputVariable<FlatNode.Runtime.VectorComponent>();
                                setVector3ComponentNode.targetComponentInput.targetNodeId = targetNodeId;
                                setVector3ComponentNode.targetComponentInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                setVector3ComponentNode.targetComponentInput = new NodeInputVariable<FlatNode.Runtime.VectorComponent>();
                                setVector3ComponentNode.targetComponentInput.value = (FlatNode.Runtime.VectorComponent)Enum.Parse(typeof(FlatNode.Runtime.VectorComponent), valueString);
                            }
                        }
                        else if (inputPortFieldName == "valueInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                setVector3ComponentNode.valueInput = new NodeInputVariable<float>();
                                setVector3ComponentNode.valueInput.targetNodeId = targetNodeId;
                                setVector3ComponentNode.valueInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                setVector3ComponentNode.valueInput = new NodeInputVariable<float>();
                                setVector3ComponentNode.valueInput.value = float.Parse(valueString);
                            }
                        }
                    }
                },
                {
                    typeof(TestNode),(nodeInstance, inputFieldInfo) =>
                    {
                        TestNode testNode = nodeInstance as TestNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "damageInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.damageInputVariable = new NodeInputVariable<float>();
                                testNode.damageInputVariable.targetNodeId = targetNodeId;
                                testNode.damageInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                testNode.damageInputVariable = new NodeInputVariable<float>();
                                testNode.damageInputVariable.value = float.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "displacementDistanceVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.displacementDistanceVariable = new NodeInputVariable<float>();
                                testNode.displacementDistanceVariable.targetNodeId = targetNodeId;
                                testNode.displacementDistanceVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                testNode.displacementDistanceVariable = new NodeInputVariable<float>();
                                testNode.displacementDistanceVariable.value = float.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "displayTextInputVariable")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.displayTextInputVariable = new NodeInputVariable<bool>();
                                testNode.displayTextInputVariable.targetNodeId = targetNodeId;
                                testNode.displayTextInputVariable.targetPortId = targetPortId;
                            }
                            else
                            {
                                testNode.displayTextInputVariable = new NodeInputVariable<bool>();
                                testNode.displayTextInputVariable.value = Boolean.Parse(valueString);
                            }
                        }
                        else if (inputPortFieldName == "skillDirection")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.skillDirection = new NodeInputVariable<UnityEngine.Vector3>();
                                testNode.skillDirection.targetNodeId = targetNodeId;
                                testNode.skillDirection.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "directionSpace")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.directionSpace = new NodeInputVariable<UnityEngine.Space>();
                                testNode.directionSpace.targetNodeId = targetNodeId;
                                testNode.directionSpace.targetPortId = targetPortId;
                            }
                            else
                            {
                                testNode.directionSpace = new NodeInputVariable<UnityEngine.Space>();
                                testNode.directionSpace.value = (UnityEngine.Space)Enum.Parse(typeof(UnityEngine.Space), valueString);
                            }
                        }
                        else if (inputPortFieldName == "collsiionMode")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.collsiionMode = new NodeInputVariable<UnityEngine.CollisionDetectionMode>();
                                testNode.collsiionMode.targetNodeId = targetNodeId;
                                testNode.collsiionMode.targetPortId = targetPortId;
                            }
                            else
                            {
                                testNode.collsiionMode = new NodeInputVariable<UnityEngine.CollisionDetectionMode>();
                                testNode.collsiionMode.value = (UnityEngine.CollisionDetectionMode)Enum.Parse(typeof(UnityEngine.CollisionDetectionMode), valueString);
                            }
                        }
                        else if (inputPortFieldName == "bulletObject")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.bulletObject = new NodeInputVariable<UnityEngine.GameObject>();
                                testNode.bulletObject.targetNodeId = targetNodeId;
                                testNode.bulletObject.targetPortId = targetPortId;
                            }
                        }
                        else if (inputPortFieldName == "bulletRotation")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                testNode.bulletRotation = new NodeInputVariable<UnityEngine.Quaternion>();
                                testNode.bulletRotation.targetNodeId = targetNodeId;
                                testNode.bulletRotation.targetPortId = targetPortId;
                            }
                        }
                    }
                },
                {
                    typeof(GetVariableNode),(nodeInstance, inputFieldInfo) =>
                    {
                        GetVariableNode getVariableNode = nodeInstance as GetVariableNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "getVariableWrapperInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                getVariableNode.getVariableWrapperInput = new NodeInputVariable<FlatNode.Runtime.VariableWrapper>();
                                getVariableNode.getVariableWrapperInput.targetNodeId = targetNodeId;
                                getVariableNode.getVariableWrapperInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                getVariableNode.getVariableWrapperInput = new NodeInputVariable<FlatNode.Runtime.VariableWrapper>();
                                int variableId = int.Parse(valueString);
                                VariableWrapper variableWrapper = variableId;
                                getVariableNode.getVariableWrapperInput.value = variableWrapper;
                            }
                        }
                    }
                },
                {
                    typeof(SetVariableNode),(nodeInstance, inputFieldInfo) =>
                    {
                        SetVariableNode setVariableNode = nodeInstance as SetVariableNode;
                        string inputPortFieldName = inputFieldInfo.FieldName;
                        int targetNodeId = inputFieldInfo.TargetNodeId;
                        int targetPortId = inputFieldInfo.TargetPortId;
                        string valueString = inputFieldInfo.ValueString;
                        
                        if (inputPortFieldName == "setVariableWrapperInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                setVariableNode.setVariableWrapperInput = new NodeInputVariable<FlatNode.Runtime.VariableWrapper>();
                                setVariableNode.setVariableWrapperInput.targetNodeId = targetNodeId;
                                setVariableNode.setVariableWrapperInput.targetPortId = targetPortId;
                            }
                            else
                            {
                                setVariableNode.setVariableWrapperInput = new NodeInputVariable<FlatNode.Runtime.VariableWrapper>();
                                int variableId = int.Parse(valueString);
                                VariableWrapper variableWrapper = variableId;
                                setVariableNode.setVariableWrapperInput.value = variableWrapper;
                            }
                        }
                        else if (inputPortFieldName == "valueVariableInput")
                        {
                            if (targetNodeId >= 0 && targetPortId >= 0)
                            {
                                setVariableNode.valueVariableInput = new NodeInputVariable<FlatNode.Runtime.NodeVariable>();
                                setVariableNode.valueVariableInput.targetNodeId = targetNodeId;
                                setVariableNode.valueVariableInput.targetPortId = targetPortId;
                            }
                        }
                    }
                },
            };
        }
        
        private static Dictionary<Type, Action<NodeBase>> nodeOutputInitFuncDictionary;
        private static void InitOutputFuncsDictionary()
        {
            if (nodeOutputInitFuncDictionary != null)
            {
                return;
            }
            
            nodeOutputInitFuncDictionary = new Dictionary<Type, Action<NodeBase>>
            {
                {
                    typeof(OnJoyStickDragNode),nodeInstance =>
                    {
                        OnJoyStickDragNode onJoyStickDragNode = nodeInstance as OnJoyStickDragNode;
                        onJoyStickDragNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            onJoyStickDragNode.GetJoyStickInput,
                        };
                    }
                },
                {
                    typeof(OnJoyStickUpNode),nodeInstance =>
                    {
                        OnJoyStickUpNode onJoyStickUpNode = nodeInstance as OnJoyStickUpNode;
                        onJoyStickUpNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            onJoyStickUpNode.GetJoyStickInput,
                        };
                    }
                },
                {
                    typeof(IntComparerNode),nodeInstance =>
                    {
                        IntComparerNode intComparerNode = nodeInstance as IntComparerNode;
                        intComparerNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            intComparerNode.GetCompareResult,
                        };
                    }
                },
                {
                    typeof(InverseDirectionNode),nodeInstance =>
                    {
                        InverseDirectionNode inverseDirectionNode = nodeInstance as InverseDirectionNode;
                        inverseDirectionNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            inverseDirectionNode.GetInverseDirection,
                        };
                    }
                },
                {
                    typeof(JoyStickInputConvertNode),nodeInstance =>
                    {
                        JoyStickInputConvertNode joyStickInputConvertNode = nodeInstance as JoyStickInputConvertNode;
                        joyStickInputConvertNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            joyStickInputConvertNode.GetConvertedVector3,
                        };
                    }
                },
                {
                    typeof(NormalizeVectorNode),nodeInstance =>
                    {
                        NormalizeVectorNode normalizeVectorNode = nodeInstance as NormalizeVectorNode;
                        normalizeVectorNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            normalizeVectorNode.GetNormalizedVector2,
                            normalizeVectorNode.GetNormalizedVector3,
                        };
                    }
                },
                {
                    typeof(QuadraticBezierPathNode),nodeInstance =>
                    {
                        QuadraticBezierPathNode quadraticBezierPathNode = nodeInstance as QuadraticBezierPathNode;
                        quadraticBezierPathNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            quadraticBezierPathNode.GetPathResultPoints,
                        };
                    }
                },
                {
                    typeof(RandomFloatNode),nodeInstance =>
                    {
                        RandomFloatNode randomFloatNode = nodeInstance as RandomFloatNode;
                        randomFloatNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            randomFloatNode.GetRandomFloat,
                        };
                    }
                },
                {
                    typeof(VectorAddNode),nodeInstance =>
                    {
                        VectorAddNode vectorAddNode = nodeInstance as VectorAddNode;
                        vectorAddNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            vectorAddNode.GetResult,
                        };
                    }
                },
                {
                    typeof(VectorScalarNode),nodeInstance =>
                    {
                        VectorScalarNode vectorScalarNode = nodeInstance as VectorScalarNode;
                        vectorScalarNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            vectorScalarNode.GetScalarVector2,
                            vectorScalarNode.GetScalarVector3,
                        };
                    }
                },
                {
                    typeof(SetVector3ComponentNode),nodeInstance =>
                    {
                        SetVector3ComponentNode setVector3ComponentNode = nodeInstance as SetVector3ComponentNode;
                        setVector3ComponentNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            setVector3ComponentNode.GetVector3Result,
                        };
                    }
                },
                {
                    typeof(TestNode),nodeInstance =>
                    {
                        TestNode testNode = nodeInstance as TestNode;
                        testNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            testNode.GetNeutralizedDamagedVariable,
                            testNode.GetDamagedVariable,
                            testNode.TypeTestOutput,
                        };
                    }
                },
                {
                    typeof(GetVariableNode),nodeInstance =>
                    {
                        GetVariableNode getVariableNode = nodeInstance as GetVariableNode;
                        getVariableNode.outputPortFuncs = new Func<NodeVariable>[]
                        {
                            getVariableNode.GetValue,
                        };
                    }
                },
            };
        }
    }
}
