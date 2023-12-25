import { LitElement, css, html } from 'lit';
import { customElement, property, query } from 'lit/decorators.js';
import { DialogComponent } from './dialog-component';

@customElement('light-webapi-component')
export class LightWebApiComponent extends LitElement {
  protected createRenderRoot() {
    return this;
  }

  @property()
  name?: string = 'World';

  @property()
  inputName: string = '';

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
    const styles = css`
      input#name1 {
        color: red;
      }
    `;
    return html`
      <style>
        ${styles}
      </style>
      <p>Light Hello, ${this.name}!</p>
      <input
        type="text"
        placeholder="name"
        id=${this.inputName}
        name=${this.inputName}
        .value="${this.name}"
      />
      <button type="button" @click=${this._onClick}>検索</button>
      <button type="button" @click=${this.openDialog}>ダイアログ</button>
      <dialog-component id="dialog"></dialog-component>
    `;
  }
}
