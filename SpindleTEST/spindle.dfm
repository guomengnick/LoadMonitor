object Form1: TForm1
  Left = 0
  Top = 0
  Caption = #20027#36600#26356#26032#38622
  ClientHeight = 154
  ClientWidth = 415
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object CheckBox_UpdateSpindleInfo: TCheckBox
    Left = 8
    Top = 32
    Width = 407
    Height = 95
    Caption = #20027#36600#27169#25836#36939#36681
    Checked = True
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -43
    Font.Name = 'Tahoma'
    Font.Style = []
    ParentFont = False
    State = cbChecked
    TabOrder = 0
    OnClick = CheckBox_UpdateSpindleInfoClick
  end
  object UpdateTimer: TTimer
    Interval = 5000
    Left = 487
    Top = 151
  end
  object Timer1: TTimer
    Enabled = False
    Interval = 3000
    Left = 472
    Top = 80
  end
end
