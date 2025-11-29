import { Component, OnInit, inject, signal, computed } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ShortenedUrl } from "../models/shortened-url.model";
import { AuthService } from "../shared/services/auth.service";
import { UrlService } from "../shared/services/url.service";

@Component({
  selector: 'app-url-table',
  templateUrl: './url-table.component.html',
  styleUrls: ['./url-table.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink]
})
export class UrlTableComponent implements OnInit {
  private urlService = inject(UrlService);
  private authService = inject(AuthService);

  urls = signal<ShortenedUrl[]>([]);
  newUrl = signal('');
  errorMessage = signal('');
  isLoggedIn = signal(false);
  private userId = signal<string | null>(null);
  private isAdmin = signal(false);

  ngOnInit(): void {
    this.isLoggedIn.set(this.authService.isLoggedIn());
    if (this.isLoggedIn()) {
      this.userId.set(this.authService.getUserId());
      this.isAdmin.set(this.authService.isAdmin());
    }
    this.loadUrls();
  }

  loadUrls(): void {
    this.urlService.getUrls().subscribe(data => {
      this.urls.set(data);
    });
  }

  shortenUrl(): void {
    const url = this.newUrl().trim();
    
    if (url === '') {
      this.errorMessage.set('URL cannot be empty.');
      return;
    }
    if (this.urls().some(u => u.longUrl === url)) {
      this.errorMessage.set('This URL has already been shortened.');
      return;
    }

    this.urlService.shortenUrl(url).subscribe({
      next: (newUrl) => {
        this.urls.update(urls => [...urls, newUrl]);
        this.newUrl.set('');
        this.errorMessage.set('');
      },
      error: (err) => {
        if (err.error && typeof err.error === 'string') {
          this.errorMessage.set(err.error);
        } else {
          this.errorMessage.set('An error occurred while shortening the URL.');
        }
        console.error(err);
      }
    });
  }

  deleteUrl(id: string): void {
    this.urlService.deleteUrl(id).subscribe(() => {
      this.urls.update(urls => urls.filter(u => u.id !== id));
    });
  }

  canDelete(url: ShortenedUrl): boolean {
    return this.isAdmin();
  }
}

