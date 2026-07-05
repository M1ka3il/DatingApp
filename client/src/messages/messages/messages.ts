import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MessageService } from '../../core/services/message-service';
import { ToastService } from '../../core/services/toast-service';
import { Message } from '../../core/models/message';
import { Pagination } from '../../core/models/pagination';

@Component({
  selector: 'app-messages',
  imports: [RouterLink, DatePipe],
  templateUrl: './messages.html',
  styleUrl: './messages.css',
})
export class Messages implements OnInit {
  private messageService = inject(MessageService);
  private toast = inject(ToastService);

  protected messages = signal<Message[]>([]);
  protected pagination = signal<Pagination | undefined>(undefined);
  protected container = signal('Inbox');
  protected pageNumber = 1;
  protected pageSize = 5;
  protected loading = signal(false);

  protected readonly containers = ['Inbox', 'Outbox', 'Unread'];

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    this.loading.set(true);
    this.messageService.getMessages(this.container(), this.pageNumber, this.pageSize).subscribe({
      next: (result) => {
        this.messages.set(result.items ?? []);
        this.pagination.set(result.pagination);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  setContainer(container: string) {
    if (this.container() === container) return;
    this.container.set(container);
    this.pageNumber = 1;
    this.loadMessages();
  }

  changePage(page: number) {
    const totalPages = this.pagination()?.totalPages ?? 1;
    if (page < 1 || page > totalPages || page === this.pageNumber) return;
    this.pageNumber = page;
    this.loadMessages();
  }

  // In Outbox we show the recipient, otherwise the sender.
  isOutbox() {
    return this.container() === 'Outbox';
  }

  partnerId(message: Message) {
    return this.isOutbox() ? message.recipientId : message.senderId;
  }

  partnerName(message: Message) {
    return this.isOutbox() ? message.recipientUsername : message.senderUsername;
  }

  partnerPhoto(message: Message) {
    return (this.isOutbox() ? message.recipientPhotoUrl : message.senderPhotoUrl)
      || '/male_defaultAvatar_vecteezy.jpg';
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        this.messages.update((m) => m.filter((x) => x.id !== id));
        this.toast.success('Message deleted');
      },
    });
  }
}
