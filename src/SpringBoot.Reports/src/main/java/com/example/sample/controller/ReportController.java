package com.example.sample.controller;

import org.springframework.http.ContentDisposition;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.example.sample.service.JasperReportService;

@RestController
@RequestMapping("/api/reports")
public class ReportController {
    private final JasperReportService jasperReportService;

    public ReportController(JasperReportService jasperReportService) {
        this.jasperReportService = jasperReportService;
    }

    @GetMapping("/a4")
    public ResponseEntity<byte[]> generateA4Report() {
        byte[] reportBytes = jasperReportService.generateA4Report();
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_PDF);
        headers.setContentDisposition(ContentDisposition.attachment().filename("a4_report.pdf").build());
        return new ResponseEntity<>(reportBytes, headers, HttpStatus.OK);
    }
}
