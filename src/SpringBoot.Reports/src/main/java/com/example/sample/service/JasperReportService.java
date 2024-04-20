package com.example.sample.service;

import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Map;

import org.springframework.stereotype.Service;

import net.sf.jasperreports.engine.JREmptyDataSource;
import net.sf.jasperreports.engine.JRException;
import net.sf.jasperreports.engine.JasperCompileManager;
import net.sf.jasperreports.engine.JasperExportManager;
import net.sf.jasperreports.engine.JasperFillManager;
import net.sf.jasperreports.engine.JasperPrint;
import net.sf.jasperreports.engine.JasperReport;

@Service
public class JasperReportService {
    private static final String REPORT_TEMPLATE = "a4_report.jrxml";

    public byte[] generateA4Report() {
        try {
            JasperReport jasperReport = compileReport(REPORT_TEMPLATE);
            JasperPrint jasperPrint = fillReport(jasperReport, new HashMap<>());
            return exportReportToPdf(jasperPrint);
        } catch (JRException e) {
            throw new RuntimeException("Error generating A4 report", e);
        }
    }

    private JasperReport compileReport(String reportTemplate) throws JRException {
        InputStream reportStream = getClass().getResourceAsStream("/" + reportTemplate);
        return JasperCompileManager.compileReport(reportStream);
    }

    private JasperPrint fillReport(JasperReport jasperReport, Map<String, Object> parameters) throws JRException {
        return JasperFillManager.fillReport(jasperReport, parameters, new JREmptyDataSource());
    }

    private byte[] exportReportToPdf(JasperPrint jasperPrint) throws JRException {
        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
        JasperExportManager.exportReportToPdfStream(jasperPrint, outputStream);
        return outputStream.toByteArray();
    }
}
