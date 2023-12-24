import { LitElement, css, html } from 'lit';
import { customElement, property, query } from 'lit/decorators.js';
import { DialogComponent } from './dialog-component';

@customElement('webapi-component')
export class WebApiComponent extends LitElement {
  static styles = css`
    :host {
      color: red;
    }
  `;

  @property()
  name?: string = 'World';

  private async _onClick() {
    if (this.name == null) {
      return;
    }
    const response = await fetch('https://localhost:7035/Simple', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ input: this.name }),
    });
    const result = await response.json();
    console.log(result);
  }

  @query('#dialog')
  private dialog!: DialogComponent;

  private openDialog() {
    this.dialog.open();
  }

  render() {
    return html`
      <p>Hello, ${this.name}!</p>
      <input type="text" .value="${this.name}" />
      <button @click=${this._onClick} part="button">検索</button>
      <button @click=${this.openDialog}>ダイアログ</button>
      <dialog-component id="dialog"></dialog-component>
    `;
  }
}
