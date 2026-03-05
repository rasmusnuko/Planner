declare global {
  namespace App {
    interface Locals {
      user: {
        id: number;
        username: string;
        display_name: string;
        color: string;
      } | null;
    }
    interface PageData {
      user?: {
        id: number;
        username: string;
        display_name: string;
        color: string;
      } | null;
    }
  }
}

export {};
