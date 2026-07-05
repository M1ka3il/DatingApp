import { Component, OnInit, computed, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../core/services/member';
import { MessageService } from '../../core/services/message-service';
import { AccountService } from '../../core/services/account-service.service';
import { ToastService } from '../../core/services/toast-service';
import { Member } from '../../core/models/member';
import { Message } from '../../core/models/message';
import { DragDrop } from '../../core/directives/drag-drop';

@Component({
  selector: 'app-member-detail',
  imports: [RouterLink, DragDrop, FormsModule, DatePipe],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.css',
})
export class MemberDetail implements OnInit {
  private memberService = inject(MemberService);
  private messageService = inject(MessageService);
  private accountService = inject(AccountService);
  private toast = inject(ToastService);

  id = input.required<string>();
  protected member = signal<Member | undefined>(undefined);
  protected uploading = signal(false);

  protected messages = signal<Message[]>([]);
  protected newMessage = '';
  protected currentUserId = computed(() => this.accountService.currentUser()?.id);

  protected isCurrentUser = computed(
    () => this.accountService.currentUser()?.id === this.member()?.id
  );

  // Show the chat thread when viewing another member while logged in.
  protected canMessage = computed(
    () => !!this.accountService.currentUser() && !this.isCurrentUser()
  );

  ngOnInit() {
    this.loadMember();
    if (this.accountService.currentUser()) {
      this.loadThread();
    }
  }

  loadMember() {
    this.memberService.getMember(this.id()).subscribe({
      next: (member) => this.member.set(member),
    });
  }

  loadThread() {
    this.messageService.getMessageThread(this.id()).subscribe({
      next: (messages) => this.messages.set(messages),
    });
  }

  sendMessage() {
    const content = this.newMessage.trim();
    if (!content) return;
    this.messageService.sendMessage(this.id(), content).subscribe({
      next: (message) => {
        this.messages.update((m) => [...m, message]);
        this.newMessage = '';
      },
    });
  }

  onFilesDropped(files: FileList) {
    this.uploadFile(files[0]);
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.uploadFile(input.files[0]);
      input.value = '';
    }
  }

  private uploadFile(file: File) {
    this.uploading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: (photo) => {
        const member = this.member();
        if (member) {
          member.photos.push(photo);
          if (photo.isMain) member.imageUrl = photo.url;
          this.member.set({ ...member });
        }
        this.toast.success('Photo uploaded');
        this.uploading.set(false);
      },
      error: () => this.uploading.set(false),
    });
  }

  setMainPhoto(photoId: number) {
    this.memberService.setMainPhoto(photoId).subscribe({
      next: () => {
        const member = this.member();
        if (member) {
          member.photos.forEach((p) => (p.isMain = p.id === photoId));
          member.imageUrl = member.photos.find((p) => p.id === photoId)?.url ?? member.imageUrl;
          this.member.set({ ...member });
        }
        this.toast.success('Main photo updated');
      },
    });
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        const member = this.member();
        if (member) {
          member.photos = member.photos.filter((p) => p.id !== photoId);
          this.member.set({ ...member });
        }
        this.toast.success('Photo deleted');
      },
    });
  }
}
