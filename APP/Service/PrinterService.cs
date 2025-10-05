using APP.Database;
using APP.Models.Database;
using APP.Models.Printer;
using bpac;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Service;

public class PrinterService
{
    private readonly AppDbContext _db;
    public PrinterService(AppDbContext db)
    {
        _db = db;
    }
    public ICollection<PrinterSetting> GetAllPrinter()
    {
        return _db.PrinterSetting.Select(p=>p).ToList();
    }
    public async Task<bool> UpdatePrinter(PrinterSetting printerSetting)
    {
        var data = await _db.PrinterSetting.FirstOrDefaultAsync(p => p.ModelName == printerSetting.ModelName);
        if (data == null) return false;
        var datas = _db.PrinterSetting.ToList();
        foreach (var item in datas)
        {
            item.Ischoose = 0;
        } 
        data.TimeUpdate = DateTime.Now.ToString();
        data.Ischoose = 1;
        data.Path = printerSetting.Path;
        _db.SaveChanges();
        return true;
    }
    public void Print(ModelPrint modelPrint, bool IsTest = false)
    {
        if (IsTest)
        {
            DocumentClass doc = new DocumentClass();
            string TemplatePath = Directory.GetCurrentDirectory() + "\\Resources\\TemplatePrint\\Temp.lbx";
            if (doc.Open(TemplatePath) != false)
            {
                doc.GetObject("ModelName").Text = "Model Name";
                doc.GetObject("MoldCode").Text = "Mold Code";
                doc.GetObject("Code1").Text = "QR1";
                doc.GetObject("Code2").Text = "QR2";
                doc.GetObject("Code3").Text = "QR3";
                doc.StartPrint("", PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();
            }
        }

       
    }

}
