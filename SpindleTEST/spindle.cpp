// ---------------------------------------------------------------------------

#include <vcl.h>
#include <IniFiles.hpp>
#include <random>
#include <ctime>
#include <windows.h>
#include <tchar.h>
#include <psapi.h>
#include <iostream>

#pragma hdrstop

#include "spindle.h"
// ---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm1 *Form1;

// ---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner) : TForm(Owner)
{

	UpdateTimer->Interval = 3500; // 設定為 1000ms
	UpdateTimer->OnTimer = UpdateSpindleInfo; // 設定 Timer 的觸發函數

}
// ---------------------------------------------------------------------------


void Log(const wchar_t* message)
{
	OutputDebugString(message);
}

void Log(const String& message)
{
	const wchar_t* message_w = message.w_str();
	OutputDebugString(message_w);
}



int GenerateRandomInt(int base, int range)
{
	return base + (std::rand() % (2 * range + 1)) - range;
}

double GenerateRandomDouble(double base, double range)
{
	double scale = static_cast<double>(std::rand()) / RAND_MAX;
	return base - range + scale * (2 * range);
}


String GenerateRandomStatus()
{
	int randomValue = std::rand() % 100; // 生成 0 到 99 的隨機整數

	if (randomValue < 5)
	{
		return "STOP"; // L"停止"; // 停止機率為 5%
	}
	else if (randomValue < 55)
	{
		return "Idle"; // L"怠速"; // 怠速機率為 50%
	}
	else
	{
		return "Run"; // L"運轉"; // 運轉機率為 45%
	}
}


void __fastcall TForm1::UpdateSpindleInfo(TObject *Sender)
{
	// 設定 INI 檔案的路徑
	String iniFilePath = ExtractFilePath(Application->ExeName) + "spindle_info.ini";

	// 建立 TMemIniFile，這裡不直接指定編碼
	std::unique_ptr<TMemIniFile>ini(new TMemIniFile(iniFilePath));

	try
	{
		// 隨機生成數值
		int speed = GenerateRandomInt(37000, 10); // Speed
		String status = GenerateRandomStatus(); // Status
		String internalStatus = GenerateRandomStatus(); // InternalStatus

		double busVoltage = GenerateRandomDouble(48.0, 0.1); // BusVoltage
		double current = GenerateRandomDouble(1.5, 0.1); // Current
		int motorTemperature = GenerateRandomInt(35, 0.1); // MotorTemperature
		double inverterTemperature = GenerateRandomDouble(39, 0.1); // InverterTemperature
		int power = current * busVoltage; //

		// 更新 INI 檔案內容（存儲到記憶體）
		ini->WriteInteger("Spindle", "Speed", speed);
		ini->WriteString("Spindle", "Status", status);
		ini->WriteString("Spindle", "InternalStatus", internalStatus);
		ini->WriteInteger("Spindle", "Power", power);
		ini->WriteFloat("Spindle", "BusVoltage", busVoltage);
		ini->WriteFloat("Spindle", "Current", current);
		ini->WriteInteger("Spindle", "MotorTemperature", motorTemperature);
		ini->WriteFloat("Spindle", "InverterTemperature", inverterTemperature);

		// 將記憶體內容保存到檔案（共享模式）
		TStringList *iniContent = new TStringList();
		try
		{
			// 將 TMemIniFile 的內容導出到 TStringList
			ini->GetStrings(iniContent);

			// 寫入檔案時使用共享模式
			TFileStream *fileStream = new TFileStream(iniFilePath, fmCreate | fmShareDenyNone);
			try
			{
				iniContent->SaveToStream(fileStream);
			}
			__finally
			{
				delete fileStream;
			}
		}
		__finally
		{
			delete iniContent;
		}
	}
	__finally
	{
		// 保證不留下任何未釋放的資源
	}
}


void __fastcall TForm1::CheckBox_UpdateSpindleInfoClick(TObject *Sender)
{


	if (CheckBox_UpdateSpindleInfo->Checked)
	{
		UpdateTimer->Enabled = true; // 開啟 Timer
	}
	else
	{
		UpdateTimer->Enabled = false; // 關閉 Timer
	}
}

// ---------------------------------------------------------------------------
bool CheckNotification()
{
	const wchar_t* pipeName = L"\\\\.\\pipe\\LoadMonitorPipe";
	// 嘗試連接到命名管道
	HANDLE hPipe = CreateFile(pipeName, /* 管道名稱 */ GENERIC_READ, /* 讀取權限 */ 0, /* 無共享 */ NULL,
			/* 預設安全屬性 */ OPEN_EXISTING, /* 打開已存在的管道 */ 0, /* 預設屬性 */ NULL); /* 無模板檔案 */

	bool result;
	if (hPipe != INVALID_HANDLE_VALUE)
	{
		// 读取数据
		char buffer[128] =
		{0};
		DWORD bytesRead;
		if (ReadFile(hPipe, buffer, sizeof(buffer) - 1, &bytesRead, NULL))
		{
			buffer[bytesRead] = '\0';
			String notification(buffer);
			result = notification == "1";
			Log(notification.c_str());
		}
		CloseHandle(hPipe);
	}
	return result;
}


UnicodeString loadMonitorWindowName = L""; // 全局变量保存窗口标题

void __fastcall TForm1::SpeedButtonLoadMonitorClick(TObject *Sender)
{
	if (!loadMonitorWindowName.IsEmpty())
	{
		HWND hWnd = FindWindow(NULL, loadMonitorWindowName.c_str());
		if (hWnd)
		{
			ShowWindow(hWnd, SW_RESTORE); // 恢复窗口
			SetForegroundWindow(hWnd); // 设置为前台窗口
		}
		else
		{
			ShowMessage(L"無法找到 LoadMonitor 窗口！");
			// Log(ERROR,
		}
	}
	else
	{
		ShowMessage(L"LoadMonitor 尚未運行！");
		// Log(ERROR,
	}
}


bool IsProcessRunning(const wchar_t* processName)
{
	DWORD processes[1024], cbNeeded, cProcesses;
	if (!EnumProcesses(processes, sizeof(processes), &cbNeeded))
	{

		Log(L"无法枚举进程！");
		return false;
	}

	// 计算进程数
	cProcesses = cbNeeded / sizeof(DWORD);

	for (unsigned int i = 0; i < cProcesses; i++)
	{
		if (processes[i] != 0) // 忽略无效的进程ID
		{
			HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE,
			processes[i]);
			if (hProcess)
			{
				TCHAR exeName[MAX_PATH] =
				{0};
				if (GetModuleBaseName(hProcess, NULL, exeName, sizeof(exeName) / sizeof(TCHAR)))
				{
					UnicodeString log = L"NAME: " + UnicodeString(exeName) + L" (PID: " +
							UnicodeString(processes[i]) + L")";
					Log(log.c_str());
				}
				else
				{
					UnicodeString log = L"無法獲取進程名稱 (PID: " + UnicodeString(processes[i]) + L")";
					Log(log.c_str());
				}
				CloseHandle(hProcess);
			}
			else
			{
				UnicodeString log = L"無法打開進程 (PID: " + UnicodeString(processes[i]) + L")，錯誤碼: " +
						UnicodeString(GetLastError());
				Log(log.c_str());
			}
		}
	}

	std::wcout << L"查找完成。" << std::endl;
	return false; // 未找到进程
}



// 判断窗口标题是否包含目标字符串
bool ContainsText(HWND hWnd, const wchar_t* searchText)
{
	wchar_t windowText[256];
	if (GetWindowText(hWnd, windowText, sizeof(windowText) / sizeof(wchar_t)))
	{
		std::wstring title(windowText);
		if (title.find(searchText) != std::wstring::npos) // 判断是否包含子字符串
		{
			return true;
		}
	}
	return false;
}

// 查找包含任意一个子字符串的窗口
bool IsLoadMonitorRunning()
{
	const wchar_t* targets[] =
	{L"Usage Monitor", L"使用率監控", L"使用率监控"};

	// 遍历所有顶层窗口
	HWND hWnd = FindWindow(NULL, NULL); // 从第一个窗口开始
	while (hWnd)
	{
		for (int i = 0; i < sizeof(targets) / sizeof(targets[0]); i++) // 使用索引遍历 targets
		{
			if (ContainsText(hWnd, targets[i]))
			{
				return true; // 有找到監控系統的視窗
			}
		}
		hWnd = GetNextWindow(hWnd, GW_HWNDNEXT); // 获取下一个窗口
	}
	return false;
}

UnicodeString GetLoadMonitorWindowName()
{
	const wchar_t* targets[] =
	{L"Usage Monitor", L"使用率監控", L"使用率监控"};
	HWND hWnd = FindWindow(NULL, NULL); // 从第一个窗口开始

	while (hWnd)
	{
		for (int i = 0; i < sizeof(targets) / sizeof(targets[0]); i++)
		{
			wchar_t windowText[256] =
			{0};
			if (GetWindowText(hWnd, windowText, sizeof(windowText) / sizeof(wchar_t)))
			{
				std::wstring title(windowText);
				if (title.find(targets[i]) != std::wstring::npos) // 判断是否包含子字符串
				{
					return UnicodeString(windowText); // 返回找到的窗口标题
				}
			}
		}
		hWnd = GetNextWindow(hWnd, GW_HWNDNEXT); // 获取下一个窗口
	}
	return L""; // 如果未找到，返回空字符串
}



// ---------------------------------------------------------------------------
void __fastcall TForm1::Button1Click(TObject *Sender)
{
	loadMonitorWindowName = GetLoadMonitorWindowName();
	if (!loadMonitorWindowName.IsEmpty())
	{
		Button1->Caption = L"找到: " + loadMonitorWindowName;
	}
	else
	{
		Button1->Caption = L"沒找到!";
	}
}

// ---------------------------------------------------------------------------


	#include <System.IOUtils.hpp>
void LaunchLoadMonitor()
{

	UnicodeString exe =
			L"C:\Users\\user\\source\\repos\\LoadMonitor\\LoadMonitor\\bin\\Debug\\net8.0-windows\\LoadMonitor.exe";
	exe = ".\\LoadMonitor\\LoadMonitor.exe";
	exe = ExtractFilePath(Application->ExeName) + "LoadMonitor.exe";
  exe = "C:\\Program1\\GAM320AT\\LoadMonitor\\LoadMonitor.exe";



String kMonitorExePath = TPath::Combine(TPath::GetFullPath("C:\\Program1\\GAM320AT\\Gam320\\..\\bin\\"), "..\\LoadMonitor\\LoadMonitor.exe");
//kMonitorExePath = TPath::Canonicalize(kMonitorExePath);
	 exe =              kMonitorExePath;

	UnicodeString modelType = L"GAM330";
	modelType = L"GAM330";
	modelType = L"GAM336AD";
	modelType = L"GAM380AT";
  modelType = "";
	// 判斷文件是否存在
	if (!FileExists(exe))
	{
		Log(L"檔案不存在: " + exe);
		return; // 文件不存在，直接返回
	}

	Log(L"未啟動監控軟件，手動啟動，啟動位置:" + exe);

	HINSTANCE hInst = ShellExecute(NULL, /* 父視窗句柄 */ L"open", /* 操作類型：打開 */ exe.c_str(),
			/* 可執行文件 */ modelType.c_str(), /* 命令行參數 */ NULL, /* 工作目錄 */ SW_SHOWMINIMIZED  /* 視窗顯示方式 */);

	if (reinterpret_cast<int>(hInst) > 32)
	{
		// Log(INFO,)
	}
	else
	{
		// Log(ERROR,
	}
}

// 讀取LoadMonitor狀態
bool last_load_monitor_result_ = false;

void __fastcall TForm1::Timer1Timer(TObject *Sender)
{
	loadMonitorWindowName = GetLoadMonitorWindowName();
	// 如果有找到監控視窗名稱，就顯示'SpeedButton'按鈕
	this->SpeedButtonLoadMonitor->Visible = !loadMonitorWindowName.IsEmpty();
	if (!this->SpeedButtonLoadMonitor->Visible)
	{
		LaunchLoadMonitor();
		return;
	}

	// 獲取監控系統的通知狀態
	bool load_monitor_result = CheckNotification();

	// 只有當狀態發生變化時才執行
	if (load_monitor_result != last_load_monitor_result_)
	{
		if (load_monitor_result)
		{
			SpeedButtonLoadMonitor->Glyph->LoadFromFile(".\\Doc\\53_53_Original_1.bmp"); // 有通知時的圖案
		}
		else
		{
			SpeedButtonLoadMonitor->Glyph->LoadFromFile(".\\Doc\\53_53_Original.bmp"); // 無通知時的圖案
		}
	}

	// 更新狀態
	last_load_monitor_result_ = load_monitor_result;

}
// ---------------------------------------------------------------------------

