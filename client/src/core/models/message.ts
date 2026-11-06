export interface Message {
  id: number;
  senderId: string;
  senderUsername: string;
  senderPhotoUrl?: string | null;
  recipientId: string;
  recipientUsername: string;
  recipientPhotoUrl?: string | null;
  content: string;
  dateRead?: string | null;
  messageSent: string;
}
