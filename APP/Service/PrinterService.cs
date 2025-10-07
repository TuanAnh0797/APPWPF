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
            string TemplatePath = Directory.GetCurrentDirectory() + "\\Resources\\TemplatePrint\\temp2.lbx";
            if (doc.Open(TemplatePath) != false)
            {
                doc.StartPrint("", PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();
            }
        }
        else
        {
            DocumentClass doc = new DocumentClass();
            string TemplatePath = Directory.GetCurrentDirectory() + "\\Resources\\TemplatePrint\\temp2.lbx";
            if (doc.Open(TemplatePath) != false)
            {
                doc.GetObject("Barcode").Text = modelPrint.MaterialCode;
                doc.GetObject("Day").Text = modelPrint.Day;
                doc.GetObject("Month").Text = modelPrint.Month;
                doc.GetObject("Year").Text = modelPrint.Year;
                doc.GetObject("Shift").Text = modelPrint.Shift;
                doc.GetObject("Mold").Text = modelPrint.Mold;
                doc.GetObject("Hour").Text = modelPrint.Hour;
                doc.GetObject("Model").Text = modelPrint.Model;
                doc.GetObject("Quantity").Text = modelPrint.Quantity;
                doc.GetObject("MaterialName").Text = modelPrint.MaterialName;
                doc.GetObject("MaterialCode").Text = modelPrint.MaterialCode;
                doc.GetObject("NameError").Text = modelPrint.NameError;
                doc.GetObject("Person").Text = modelPrint.Person;
                doc.GetObject("Reason").Text = modelPrint.Reason;
                doc.GetObject("Color").Text = modelPrint.MaterialColor;
                doc.StartPrint("", PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();
            }
        }

       
    }

}
