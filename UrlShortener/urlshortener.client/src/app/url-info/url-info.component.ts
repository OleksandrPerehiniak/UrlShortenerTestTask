import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UrlService } from '../shared/services/url.service';
import { CommonModule } from '@angular/common';
import { ShortenedUrl } from '../models/shortened-url.model';

@Component({
  selector: 'app-url-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './url-info.component.html',
  styleUrls: ['./url-info.component.css']
})
export class UrlInfoComponent implements OnInit {
  url: ShortenedUrl | null = null;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private urlService: UrlService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const code = this.route.snapshot.paramMap.get('code');
    if (code) {
      this.urlService.getUrlInfo(code).subscribe({
        next: (data) => {
          this.url = data;
        },
        error: (err) => {
          this.error = 'Could not retrieve URL information.';
          console.error(err);
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/urls']);
  }
}

