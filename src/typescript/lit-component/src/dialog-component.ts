import { LitElement, html, css } from 'lit';
import { customElement } from 'lit/decorators.js';

@customElement('dialog-component')
export class DialogComponent extends LitElement {
  static styles = css`
    :host {
      display: none;
    }
  `;

  public open() {
    this.style.display = 'block';
  }

  close() {
    this.style.display = 'none';
  }

  render() {
    return html`
      <div>
        <p>Hello, this is a dialog!</p>
        <button type="button" @click="${this.close}">Close Dialog</button>
      </div>
    `;
  }
}
