import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  dataLocalUrl: any

  constructor(private http: HttpClient, private sanitizer: DomSanitizer) { }

  ngOnInit(): void {
    this.showPdfFile();
  }

  // This method is calling API endpoint to get PDF file
  getPDF(): Observable<any> {
    var url: string = 'http://localhost:7071/api/pdffile';
    return this.http.get(url, { responseType: 'blob' as 'json' });
  }

  // this method is reading rsponce received in above mentod from API
  private showPdfFile() {
    this.getPDF().subscribe(
      (response) => {
        var pdfFile = new Blob([response], { type: 'application/pdf' });
        var path = URL.createObjectURL(pdfFile);
        this.dataLocalUrl = this.sanitizer.bypassSecurityTrustResourceUrl(path);
      },
      error => {
        console.log(error);
      });
  }

}
