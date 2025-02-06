//---------------------------------------------------------------------------

#ifndef spindleH
#define spindleH
//---------------------------------------------------------------------------
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
	TTimer *UpdateTimer;
	TCheckBox *CheckBox_UpdateSpindleInfo;
	TTimer *Timer1;
	void __fastcall CheckBox_UpdateSpindleInfoClick(TObject *Sender);
	void __fastcall SpeedButtonLoadMonitorClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
	__fastcall TForm1(TComponent* Owner);
	void __fastcall UpdateSpindleInfo(TObject *Sender);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
