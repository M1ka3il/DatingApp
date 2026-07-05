export interface Photo {
  id: number;
  url: string;
  isMain: boolean;
}

export interface Member {
  id: string;
  userName: string;
  imageUrl: string | null;
  photos: Photo[];
}
