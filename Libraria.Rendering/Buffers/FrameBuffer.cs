using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Libraria.Rendering {
	public class FrameBuffer : OpenGLBuffer<FrameBuffer> {
		public FrameBuffer() {
			ID = GL.GenFramebuffer();
			Bind();
		}

		public override FrameBuffer Bind() {
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
			return this;
		}

		public override FrameBuffer Unbind() {
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			return this;
		}

		public void BindRenderBuffer(RenderBuffer RBuffer, FramebufferAttachment Attachment) {
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, Attachment, RenderbufferTarget.Renderbuffer, RBuffer.ID);
		}

		public void BindTexture(Texture2D Tex, FramebufferAttachment Attachment) {
			GL.FramebufferTexture(FramebufferTarget.Framebuffer, Attachment, Tex.ID, 0);
		}

		public void DrawBuffer(DrawBufferMode Buffer) {
			GL.DrawBuffer(Buffer);
		}

		public override void Destroy() {
			GL.DeleteFramebuffer(ID);
		}
	}
}
