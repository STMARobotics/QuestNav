/*
* QUESTNAV
  https://github.com/QuestNav
* Copyright (C) 2025 QuestNav
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the MIT License as published.
*/
package gg.questnav.questnav.protos.wpilib;

import edu.wpi.first.util.protobuf.Protobuf;
import gg.questnav.questnav.protos.generated.Data;
import us.hebi.quickbuf.Descriptors;

/** WPILib Protobuf layer for FrameData Protobuf */
public class FrameDataProto
    implements Protobuf<Data.ProtobufQuestNavFrameData, Data.ProtobufQuestNavFrameData> {
  @Override
  public Class<Data.ProtobufQuestNavFrameData> getTypeClass() {
    return Data.ProtobufQuestNavFrameData.class;
  }

  @Override
  public Descriptors.Descriptor getDescriptor() {
    return Data.ProtobufQuestNavFrameData.getDescriptor();
  }

  @Override
  public Data.ProtobufQuestNavFrameData createMessage() {
    return Data.ProtobufQuestNavFrameData.newInstance();
  }

  @Override
  public Data.ProtobufQuestNavFrameData unpack(Data.ProtobufQuestNavFrameData msg) {
    return msg.clone();
  }

  @Override
  public void pack(Data.ProtobufQuestNavFrameData msg, Data.ProtobufQuestNavFrameData value) {
    msg.copyFrom(value);
  }
}
