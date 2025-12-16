import { Component, Input, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MediaItem } from '../../models/media-gallery.model';

@Component({
  selector: 'app-video-player',
  standalone: false,
  templateUrl: './video-player.component.html',
  styleUrls: ['./video-player.component.scss']
})
export class VideoPlayerComponent implements OnInit {
  @Input() video!: MediaItem;
  @ViewChild('videoElement') videoElement?: ElementRef<HTMLVideoElement>;

  isPlaying = false;
  currentTime = 0;
  duration = 0;
  volume = 1;
  isMuted = false;
  showControls = true;
  isFullscreen = false;

  ngOnInit(): void {
    this.duration = this.video.duration || 0;
  }

  togglePlay(): void {
    const video = this.videoElement?.nativeElement;
    if (!video) return;

    if (this.isPlaying) {
      video.pause();
    } else {
      video.play();
    }
    this.isPlaying = !this.isPlaying;
  }

  onTimeUpdate(): void {
    const video = this.videoElement?.nativeElement;
    if (video) {
      this.currentTime = video.currentTime;
      this.duration = video.duration;
    }
  }

  seek(time: number): void {
    const video = this.videoElement?.nativeElement;
    if (video) {
      video.currentTime = time;
      this.currentTime = time;
    }
  }

  setVolume(volume: number): void {
    const video = this.videoElement?.nativeElement;
    if (video) {
      video.volume = volume;
      this.volume = volume;
      this.isMuted = volume === 0;
    }
  }

  toggleMute(): void {
    this.isMuted = !this.isMuted;
    const video = this.videoElement?.nativeElement;
    if (video) {
      video.muted = this.isMuted;
    }
  }

  toggleFullscreen(): void {
    const container = this.videoElement?.nativeElement.parentElement;
    if (!container) return;

    if (!this.isFullscreen) {
      container.requestFullscreen();
    } else {
      document.exitFullscreen();
    }
    this.isFullscreen = !this.isFullscreen;
  }

  formatTime(seconds: number): string {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }
}
